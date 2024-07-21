namespace Bones.Api.HostedServices.Tasks.Daily;

internal abstract class DailyTaskBase(ISender sender) : TaskBase(sender)
{
    internal sealed override TaskFrequency Frequency => TaskFrequency.Daily;

    internal sealed override DateTimeOffset? GetNextRunTime()
    {
        // +/- 2 hours, 120 minutes
        return DateTimeOffset.UtcNow.AddHours(24).AddMinutes(Random.Next(241) - 120);
    }
}