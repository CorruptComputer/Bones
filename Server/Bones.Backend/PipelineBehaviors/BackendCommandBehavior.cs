using System.Diagnostics;
using Bones.Backend.Models;
using MediatR;
using Serilog;

namespace Bones.Backend.PipelineBehaviors;

internal class BackendCommandBehavior<TRequest> : BackendPipelineBehaviorBase,
    IPipelineBehavior<TRequest, BackendCommandResponse>
    where TRequest : notnull
{
    public async Task<BackendCommandResponse> Handle(TRequest request,
        RequestHandlerDelegate<BackendCommandResponse> next,
        CancellationToken cancellationToken)
    {
        Stopwatch? stopwatch = null;
        if (DebugLog)
        {
            Log.Debug($"Backend Command Started [{typeof(TRequest).FullName}] TRequest = {request.ToString()}");
            stopwatch = Stopwatch.StartNew();
        }

        Exception? exception = null;
        BackendCommandResponse? response;

        try
        {
            response = await next();
        }
        catch (Exception e)
        {
            exception = e;
            response = new()
            {
                Success = false,
                FailureReason = e.Message
            };
        }

        if (exception != null)
        {
            string exceptionName = exception.GetType().FullName ?? "No Name";
            Log.Error(
                $"Backend Command Uncaught Exception [{typeof(TRequest).FullName}] | Exception = {exceptionName} | ExceptionMessage = {exception.Message} | ExceptionStackTrace = {exception.StackTrace ?? "(none)"}");

            if (DebugLog)
            {
                Log.Debug(
                    $"Backend Command Uncaught Exception [{typeof(TRequest).FullName}] | Exception = {exceptionName} | TRequest = {request.ToString()}");
            }
        }
        else if (DebugLog && stopwatch != null)
        {
            stopwatch.Stop();
            Log.Debug(response.Success
                ? $"Backend Command Succeeded [{typeof(TRequest).FullName}] in {stopwatch.ElapsedMilliseconds}ms"
                : $"Backend Command Failed [{typeof(TRequest).FullName}] in {stopwatch.ElapsedMilliseconds}ms | Reason = {response.FailureReason ?? "(none)"}");
        }

        return response;
    }
}