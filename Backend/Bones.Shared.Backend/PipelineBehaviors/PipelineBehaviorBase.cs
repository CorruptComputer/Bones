using System.Diagnostics;
using Bones.Shared.Exceptions;
using Serilog.Events;

namespace Bones.Shared.Backend.PipelineBehaviors;

/// <summary>
///   Base pipeline behavior for MediatR
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public abstract class PipelineBehaviorBase<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    /// <summary>
    ///   Gets the basic result info from the response
    /// </summary>
    /// <param name="response"></param>
    /// <returns></returns>
    protected abstract (bool success, string? failReason, bool forbidden) GetResult(TResponse response);

    /// <summary>
    ///   Generate a failure response with the provided type
    /// </summary>
    /// <param name="failReason"></param>
    /// <returns></returns>
    protected abstract TResponse GetFailedResponse(string failReason);

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
    ///   This is called before MediatR sends the request to its handler, we pass the request along with next()
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
            // Send it to the handler
            response = await next();
        }
        catch (Exception e) when (e is not ForbiddenAccessException)
        {
            Log.Error(e, "Uncaught Exception [{RequestName}] | ExceptionMessage = {Message}", typeof(TRequest).FullName, e.Message);

            exception = e;
            response = GetFailedResponse(e.Message);
        }

        (bool success, string? failReason, bool forbidden) = GetResult(response);

        if (forbidden)
        {
            // Stop the whole call stack here, we don't want anything with this request to go any further.
            // The API Controller should catch this and return a forbidden status code.
            throw new ForbiddenAccessException();
        }

        StopDebugLog(request, success, failReason, exception);

        return response;
    }

    private void StartDebugLog(TRequest request)
    {
        if (!_debugLog)
        {
            return;
        }

        Log.Debug($"Request Started [{typeof(TRequest).FullName}] TRequest = {request.ToString()}");
        _stopwatch = Stopwatch.StartNew();
    }

    private void StopDebugLog(TRequest request, bool success, string? failReason, Exception? exception)
    {
        if (!_debugLog || _stopwatch == null)
        {
            return;
        }

        _stopwatch.Stop();
        if (exception != null)
        {
            Log.Debug($"Uncaught Exception [{typeof(TRequest).FullName}] | Exception = {exception.Message} | TRequest = {request.ToString()}");
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
        }

        Log.Debug(success
            ? $"Succeeded [{typeof(TRequest).FullName}] in {_stopwatch.ElapsedMilliseconds}ms"
            : $"Failed [{typeof(TRequest).FullName}] in {_stopwatch.ElapsedMilliseconds}ms | Reason = {failReason ?? "(none)"}");
    }
}