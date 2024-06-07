using System.Diagnostics;
using Bones.Backend.Models;
using MediatR;
using Serilog;

namespace Bones.Backend.PipelineBehaviors;

internal class BackendQueryBehavior<TRequest, TResponse> : BackendPipelineBehaviorBase,
    IPipelineBehavior<TRequest, BackendQueryResponse<TResponse>>
    where TRequest : notnull
{
    public async Task<BackendQueryResponse<TResponse>> Handle(TRequest request,
        RequestHandlerDelegate<BackendQueryResponse<TResponse>> next, CancellationToken cancellationToken)
    {
        Stopwatch? stopwatch = null;
        if (DebugLog)
        {
            Log.Debug($"Backend Query Started [{typeof(TRequest).FullName}] TRequest = {request.ToString()}");
            stopwatch = Stopwatch.StartNew();
        }

        Exception? exception = null;
        BackendQueryResponse<TResponse>? response;

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
                $"DB Query Uncaught Exception [{typeof(TRequest).FullName}] | Exception = {exceptionName} | ExceptionMessage = {exception.Message} | ExceptionStackTrace = {exception.StackTrace ?? "(none)"}");

            if (DebugLog)
            {
                Log.Debug(
                    $"DB Query Uncaught Exception [{typeof(TRequest).FullName}] | Exception = {exceptionName} | TRequest = {request.ToString()}");
            }
        }
        else if (DebugLog && stopwatch != null)
        {
            stopwatch.Stop();
            Log.Debug(response.Success
                ? $"DB Query Succeeded [{typeof(TRequest).FullName}] in {stopwatch.ElapsedMilliseconds}ms | TResponse = {response.Result?.ToString() ?? "(none)"}"
                : $"DB Query Failed [{typeof(TRequest).FullName}] in {stopwatch.ElapsedMilliseconds}ms | Reason = {response.FailureReason ?? "(none)"}");
        }

        return response;
    }
}