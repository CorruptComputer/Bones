using System.Diagnostics;
using Bones.Shared.Exceptions;
using FluentValidation;
using Serilog.Events;

namespace Bones.Shared.Backend.PipelineBehaviors;

/// <summary>
///   Base pipeline behavior for Questy
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public abstract class PipelineBehaviorBase<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> requestValidators) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    /// <summary>
    ///   Gets the basic result info from the response
    /// </summary>
    /// <param name="response"></param>
    /// <returns></returns>
    protected abstract (bool success, Dictionary<string, List<string>>? failReason, bool forbidden) GetResult(TResponse response);

    /// <summary>
    ///   Generate a failure response with the provided type
    /// </summary>
    /// <param name="failReason"></param>
    /// <returns></returns>
    protected abstract TResponse GetFailedResponse(Dictionary<string, List<string>> failReason);

    // I don't really like using these, but in this case it's safer to do so.
    // Logging the request could potentially log a password,
    // so if it's built in release mode we need to make sure that's not possible.
#if DEBUG
    private readonly bool _debugLog = Log.IsEnabled(LogEventLevel.Debug);
#else
    private readonly bool _debugLog = false;
#endif

    private Stopwatch? _stopwatch;

    /// <summary>
    ///   This is called before Questy sends the request to its handler, we pass the request along with next()
    /// </summary>
    /// <param name="request"></param>
    /// <param name="next"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        StartDebugLog(request);
        Exception? exception = null;
        TResponse? response;

        try
        {
            if (!requestValidators.Any())
            {
                response = await next();
            }
            else
            {
                ValidationContext<TRequest> context = new(request);
                Dictionary<string, List<string>> errors = requestValidators
                    .Select(x => x.Validate(context))
                    .SelectMany(x => x.Errors)
                    .Where(x => x != null)
                    .GroupBy(
                        x => x.PropertyName,
                        x => x.ErrorMessage,
                        (propertyName, errorMessages) => new
                        {
                            Key = propertyName,
                            Values = errorMessages.Distinct().ToList(),
                        })
                    .ToDictionary(x => x.Key, x => x.Values);

                if (errors.Count != 0)
                {
                    response = GetFailedResponse(errors);
                }
                else
                {
                    // Send it to the handler
                    response = await next();
                }
            }
        }
        catch (Exception e) when (e is not ForbiddenException)
        {
            Log.Error(e, "Uncaught Exception [{RequestName}] | ExceptionMessage = {Message}", typeof(TRequest).FullName, e.Message);

            exception = e;
            response = GetFailedResponse(new()
            {
                { "Internal Error", [ e.Message ] }
            });
        }

        (bool success, Dictionary<string, List<string>>? failReasons, bool forbidden) = GetResult(response);

        if (forbidden)
        {
            // Stop the whole call stack here, we don't want anything with this request to go any further.
            // The API Controller should catch this and return a forbidden status code.
            throw new ForbiddenException();
        }

        StopDebugLog(request, success, failReasons, exception);

        return response;
    }

    private void StartDebugLog(TRequest request)
    {
        if (!_debugLog)
        {
            return;
        }

        Log.Debug("Request Started [{TypeName}] TRequest = {RequestBody}", typeof(TRequest).FullName, request.ToString());
        _stopwatch = Stopwatch.StartNew();
    }

    private void StopDebugLog(TRequest request, bool success, Dictionary<string, List<string>>? failReasons, Exception? exception)
    {
        if (!_debugLog || _stopwatch == null)
        {
            return;
        }

        _stopwatch.Stop();
        if (exception != null)
        {
            Log.Debug("Uncaught Exception [{TypeName}] | Exception = {ExceptionMesssage} | TRequest = {RequestBody}", typeof(TRequest).FullName, exception.Message, request.ToString());
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
        }

        if (success)
        {
            Log.Debug("Request Succeeded [{TypeName}] | TRequest = {RequestBody}", typeof(TRequest).FullName, request.ToString());
        }
        else
        {
            Log.Debug("Request Failed [{TypeName}] | TRequest = {RequestBody} | FailReasons = {FailReasons}", typeof(TRequest).FullName, request.ToString(), failReasons?.SelectMany(x => x.Value).Aggregate((x, y) => $"{x}, {y}") ?? "(none)");
        }
    }
}