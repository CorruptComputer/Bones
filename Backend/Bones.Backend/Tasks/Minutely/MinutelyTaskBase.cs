namespace Bones.Backend.Tasks.Minutely;

public abstract class MinutelyTaskBase<T>(ILogger<T> logger, ISender sender) : TaskBase<T>(logger, sender)
{
    protected override TimeSpan? Interval { get; } = TimeSpan.FromMinutes(1);
}