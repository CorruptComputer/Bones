namespace Bones.Backend.Tasks.Startup;

public abstract class StartupTaskBase<T>(ILogger<T> logger) : TaskBase<T>(logger)
{
    protected override TimeSpan? Interval { get; init; } = null;
}