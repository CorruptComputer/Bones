using System.Diagnostics;
using Bones.Database.Models;
using MediatR;
using Serilog;

namespace Bones.Database.PipelineBehaviors;

internal class DbCommandBehavior<TRequest> : DbPipelineBehaviorBase, IPipelineBehavior<TRequest, DbCommandResponse>
    where TRequest : notnull
{
    public async Task<DbCommandResponse> Handle(TRequest request, RequestHandlerDelegate<DbCommandResponse> next,
        CancellationToken cancellationToken)
    {
        Stopwatch? stopwatch = null;
        if (DebugLog)
        {
            Log.Debug($"DB Command Started [{typeof(TRequest).FullName}] TRequest = {request.ToString()}");
            stopwatch = Stopwatch.StartNew();
        }

        Exception? exception = null;
        DbCommandResponse? response;

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
                $"DB Command Uncaught Exception [{typeof(TRequest).FullName}] | Exception = {exceptionName} | ExceptionMessage = {exception.Message} | ExceptionStackTrace = {exception.StackTrace ?? "(none)"}");

            if (DebugLog)
            {
                Log.Debug(
                    $"DB Command Uncaught Exception [{typeof(TRequest).FullName}] | Exception = {exceptionName} | TRequest = {request.ToString()}");
            }
        }
        else if (DebugLog && stopwatch != null)
        {
            stopwatch.Stop();
            Log.Debug(response.Success
                ? $"DB Command Succeeded [{typeof(TRequest).FullName}] in {stopwatch.ElapsedMilliseconds}ms"
                : $"DB Command Failed [{typeof(TRequest).FullName}] in {stopwatch.ElapsedMilliseconds}ms | Reason = {response.FailureReason ?? "(none)"}");
        }

        return response;
    }
}