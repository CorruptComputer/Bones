using System.Diagnostics;
using Serilog.Events;

namespace Bones.Shared.PipelineBehaviors;

public abstract class PipelineBehaviorBase<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    protected abstract (bool success, string? failReason) GetResult(TResponse response);
    protected abstract TResponse GetFailedResponse(string failReason);

    private readonly bool _debugLog = Log.IsEnabled(LogEventLevel.Debug);
    private Stopwatch? _stopwatch = null;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        StartDebugLog(request);
        Exception? exception = null;
        TResponse? response = default;

        try
        {
            if (request is IValidatableRequest<TResponse> val && !val.IsRequestValid())
            {
                response = GetFailedResponse("Parameters failed basic validation.");
            }

            // Skip if set by invalid above
            response ??= await next();
        }
        catch (Exception e)
        {
            exception = e;
            response = GetFailedResponse(e.Message);

            Log.Error($"Uncaught Exception [{typeof(TRequest).FullName}] | ExceptionMessage = {exception.Message}");
        }

        (bool success, string? failReason) = GetResult(response);

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
        }

        Log.Debug(success
            ? $"Succeeded [{typeof(TRequest).FullName}] in {_stopwatch.ElapsedMilliseconds}ms"
            : $"Failed [{typeof(TRequest).FullName}] in {_stopwatch.ElapsedMilliseconds}ms | Reason = {failReason ?? "(none)"}");
    }
}