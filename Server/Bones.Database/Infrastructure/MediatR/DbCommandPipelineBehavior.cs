using System.Diagnostics;
using Bones.Database.Models;
using MediatR;
using Serilog;

namespace Bones.Database.Infrastructure.MediatR;

internal class DbCommandPipelineBehavior<TRequest> : IPipelineBehavior<TRequest, DbCommandResponse>
    where TRequest : notnull
{
    public async Task<DbCommandResponse> Handle(TRequest request, RequestHandlerDelegate<DbCommandResponse> next, CancellationToken cancellationToken)
    {
        Guid requestId = Guid.NewGuid();
        bool hasException = false;
        Exception? exception = null;
        Stopwatch stopwatch = Stopwatch.StartNew();
        DbCommandResponse? response = default;

        try
        {
            Log.Debug($"DB Command Started [{requestId}] | {typeof(TRequest).FullName} | {request.ToString()}");
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
                Log.Debug($"DB Command Finished [{requestId}] | {stopwatch.ElapsedMilliseconds}ms | {response}");
            }
            else
            {
                Log.Error($"DB Command Errored [{requestId}] | {exception?.GetType().FullName ?? "No Name"} | {stopwatch.ElapsedMilliseconds}ms | {exception?.Message ?? "No Message"} | {typeof(TRequest).FullName} | {request.ToString()}");
            }
        }
    }
}