using MediatR;

namespace Bones.Backend.HostedServices.Tasks.Hourly;

internal abstract class HourlyTaskBase(ISender sender) : TaskBase(sender)
{
    internal sealed override TaskFrequency Frequency => TaskFrequency.Hourly;

    internal sealed override DateTimeOffset? GetNextRunTime()
    {
        // +/- 5 minutes, 300 seconds
        return DateTimeOffset.UtcNow.AddHours(1).AddSeconds(Random.Next(601) - 300);
    }
}