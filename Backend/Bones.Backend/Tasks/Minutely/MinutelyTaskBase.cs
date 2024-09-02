namespace Bones.Backend.Tasks.Minutely;

internal abstract class MinutelyTaskBase<T>(ILogger<T> logger, ISender sender) : TaskBase<T>(logger, sender)
{
    protected override TimeSpan? Interval { get; } = TimeSpan.FromMinutes(1);
    
    protected override Task<bool> ShouldTaskRunAsync(CancellationToken cancellationToken) => Task.FromResult(IsEnabled);
}