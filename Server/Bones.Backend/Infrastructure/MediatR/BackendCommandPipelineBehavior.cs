using System.Diagnostics;
using Bones.Backend.Models;
using MediatR;
using Serilog;

namespace Bones.Backend.Infrastructure.MediatR;

internal class BackendCommandPipelineBehavior<TRequest> : IPipelineBehavior<TRequest, CommandResponse>
    where TRequest : notnull
{
    public async Task<CommandResponse> Handle(TRequest request, RequestHandlerDelegate<CommandResponse> next, CancellationToken cancellationToken)
    {
        Guid requestId = Guid.NewGuid();
        bool hasException = false;
        Exception? exception = null;
        Stopwatch stopwatch = Stopwatch.StartNew();
        CommandResponse? response = default;

        try
        {
            Log.Debug($"Command Started [{requestId}] | {typeof(TRequest).FullName} | {request.ToString()}");
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
                Log.Debug($"Command Finished [{requestId}] | {stopwatch.ElapsedMilliseconds}ms | {response}");
            }
            else
            {
                Log.Error($"Command Errored [{requestId}] | {exception?.GetType().FullName ?? "No Name"} | {stopwatch.ElapsedMilliseconds}ms | {exception?.Message ?? "No Message"} | {typeof(TRequest).FullName} | {request.ToString()}");
            }
        }
    }
}