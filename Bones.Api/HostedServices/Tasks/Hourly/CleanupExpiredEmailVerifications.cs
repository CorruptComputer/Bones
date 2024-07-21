namespace Bones.Api.HostedServices.Tasks.Hourly;

internal class CleanupExpiredEmailVerifications(ISender sender) : HourlyTaskBase(sender)
{
    internal override string TaskName => nameof(CleanupExpiredEmailVerifications);

    internal override Task<bool> ShouldRunAsync(CancellationToken cancellationToken)
    {
        if (Running)
        {
            return Task.FromResult(false);
        }

        throw new NotImplementedException();
    }

    internal override Task RunAsync(CancellationToken cancellationToken)
    {
        // ClearExpiredEmailVerificationsDbQuery
        throw new NotImplementedException();
    }
}