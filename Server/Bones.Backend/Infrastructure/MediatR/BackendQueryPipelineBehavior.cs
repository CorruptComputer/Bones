using System.Diagnostics;
using Bones.Backend.Models;
using MediatR;
using Serilog;

namespace Bones.Backend.Infrastructure.MediatR;

internal class BackendQueryPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, QueryResponse<TResponse>>
    where TRequest : notnull
{
    public async Task<QueryResponse<TResponse>> Handle(TRequest request,
        RequestHandlerDelegate<QueryResponse<TResponse>> next, CancellationToken cancellationToken)
    {
        Guid requestId = Guid.NewGuid();
        bool hasException = false;
        Exception? exception = null;
        Stopwatch stopwatch = Stopwatch.StartNew();
        QueryResponse<TResponse>? response = default;

        try
        {
            Log.Debug($"Query Started [{requestId}] | {typeof(TRequest).FullName} | {request.ToString()}");
            response = await next();
            return response;
        }
        catch (Exception e)
        {
            hasException = true;
            exception = e;
            return new()
            {
                Success = false,
                FailureReason = e.Message
            };
        }
        finally
        {
            stopwatch.Stop();
            if (!hasException && response != null)
            {
                Log.Debug($"Query Finished [{requestId}] | {stopwatch.ElapsedMilliseconds}ms | {response}");
            }
            else
            {
                Log.Error(
                    $"Query Errored [{requestId}] | {exception?.GetType().FullName ?? "No Name"} | {stopwatch.ElapsedMilliseconds}ms | {exception?.Message ?? "No Message"} | {typeof(TRequest).FullName} | {request.ToString()}");
            }
        }
    }
}