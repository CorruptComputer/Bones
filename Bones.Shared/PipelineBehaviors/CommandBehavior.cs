using System.Diagnostics;

namespace Bones.Shared.PipelineBehaviors;

/// <summary>
///     Default pipeline behavior for Commands
/// </summary>
/// <typeparam name="TRequest">Type of the request object</typeparam>
public class CommandBehavior<TRequest> : PipelineBehaviorBase, IPipelineBehavior<TRequest, CommandResponse>
    where TRequest : notnull
{
    /// <summary>
    ///     Handles the request
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="next">Next delegate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The commands response</returns>
    public async Task<CommandResponse> Handle(TRequest request, RequestHandlerDelegate<CommandResponse> next,
        CancellationToken cancellationToken)
    {
        Stopwatch? stopwatch = null;
        if (DebugLog)
        {
            Log.Debug($"DB Command Started [{typeof(TRequest).FullName}] TRequest = {request.ToString()}");
            stopwatch = Stopwatch.StartNew();
        }

        Exception? exception = null;
        CommandResponse? response;

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