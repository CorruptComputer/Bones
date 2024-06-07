using System.Diagnostics;
using Bones.Database.Models;
using MediatR;
using Serilog;

namespace Bones.Database.PipelineBehaviors;

internal class DbQueryBehavior<TRequest, TResponse> : DbPipelineBehaviorBase,
    IPipelineBehavior<TRequest, DbQueryResponse<TResponse>>
    where TRequest : notnull
{
    public async Task<DbQueryResponse<TResponse>> Handle(TRequest request,
        RequestHandlerDelegate<DbQueryResponse<TResponse>> next, CancellationToken cancellationToken)
    {
        Stopwatch? stopwatch = null;
        if (DebugLog)
        {
            Log.Debug($"DB Query Started [{typeof(TRequest).FullName}] TRequest = {request.ToString()}");
            stopwatch = Stopwatch.StartNew();
        }

        Exception? exception = null;
        DbQueryResponse<TResponse>? response;

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