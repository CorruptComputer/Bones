using System.Diagnostics;

namespace Bones.Shared.PipelineBehaviors;

/// <summary>
///     Default pipeline behavior for Commands
/// </summary>
/// <typeparam name="TRequest">Type of the request object</typeparam>
/// <typeparam name="TResponse">Type of the response object</typeparam>
public class QueryBehavior<TRequest, TResponse> : PipelineBehaviorBase,
    IPipelineBehavior<TRequest, QueryResponse<TResponse>>
    where TRequest : notnull
{
    /// <summary>
    ///     Handles the request
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="next">Next delegate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The query response</returns>
    public async Task<QueryResponse<TResponse>> Handle(TRequest request,
        RequestHandlerDelegate<QueryResponse<TResponse>> next, CancellationToken cancellationToken)
    {
        Stopwatch? stopwatch = null;
        if (DebugLog)
        {
            Log.Debug($"DB Query Started [{typeof(TRequest).FullName}] TRequest = {request.ToString()}");
            stopwatch = Stopwatch.StartNew();
        }

        Exception? exception = null;
        QueryResponse<TResponse>? response;

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