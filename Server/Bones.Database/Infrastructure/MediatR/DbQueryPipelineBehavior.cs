using System.Diagnostics;
using Bones.Database.Models;
using MediatR;
using Serilog;

namespace Bones.Database.Infrastructure.MediatR;

internal class DbQueryPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, DbQueryResponse<TResponse>>
    where TRequest : notnull
{
    public async Task<DbQueryResponse<TResponse>> Handle(TRequest request, RequestHandlerDelegate<DbQueryResponse<TResponse>> next, CancellationToken cancellationToken)
    {
        Guid requestId = Guid.NewGuid();
        bool hasException = false;
        Exception? exception = null;
        Stopwatch stopwatch = Stopwatch.StartNew();
        DbQueryResponse<TResponse>? response = default;

        try
        {
            Log.Debug($"DB Query Started [{requestId}] | {typeof(TRequest).FullName} | {request.ToString()}");
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
                Log.Debug($"DB Query Finished [{requestId}] | {stopwatch.ElapsedMilliseconds}ms | {response}");
            }
            else
            {
                Log.Error($"DB Query Errored [{requestId}] | {exception?.GetType().FullName ?? "No Name"} | {stopwatch.ElapsedMilliseconds}ms | {exception?.Message ?? "No Message"} | {typeof(TRequest).FullName} | {request.ToString()}");
            }
        }
    }
}