namespace Bones.Backend.Tasks.Minutely;

public abstract class MinutelyTaskBase<T>(ILogger<T> logger) : TaskBase<T>(logger)
{
    protected override TimeSpan? Interval { get; init; } = TimeSpan.FromMinutes(1);
}