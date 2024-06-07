using MediatR;

namespace Bones.Backend.HostedServices.Tasks.Weekly;

internal abstract class WeeklyTaskBase(ISender sender) : TaskBase(sender)
{
    internal sealed override TaskFrequency Frequency => TaskFrequency.Weekly;

    internal sealed override DateTimeOffset? GetNextRunTime()
    {
        // +/- 3 hours, 180 minutes
        return DateTimeOffset.UtcNow.AddDays(7).AddMinutes(Random.Next(361) - 180);
    }
}