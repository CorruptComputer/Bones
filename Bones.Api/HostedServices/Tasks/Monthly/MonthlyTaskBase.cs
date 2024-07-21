namespace Bones.Api.HostedServices.Tasks.Monthly;

internal abstract class MonthlyTaskBase(ISender sender) : TaskBase(sender)
{
    internal sealed override TaskFrequency Frequency => TaskFrequency.Monthly;

    internal sealed override DateTimeOffset? GetNextRunTime()
    {
        // +/- 2 days, 48 hours
        return DateTimeOffset.UtcNow.AddMonths(1).AddHours(Random.Next(97) - 48);
    }
}