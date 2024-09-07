namespace Bones.BackgroundService.Tasks.Startup;

internal abstract class StartupTaskBase<T>(ILogger<T> logger, ISender sender) : TaskBase<T>(logger, sender)
{
    protected override TimeSpan? Interval { get; } = null;

    protected override Task<bool> ShouldTaskRunAsync(CancellationToken cancellationToken) => Task.FromResult(IsEnabled);
}