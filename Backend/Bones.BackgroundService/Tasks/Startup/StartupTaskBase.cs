namespace Bones.BackgroundService.Tasks.Startup;

internal abstract class StartupTaskBase(ISender sender) : TaskBase(sender)
{
    protected override TimeSpan Interval { get; } = TimeSpan.FromSeconds(1);
    
    protected override bool IsStartupOnlyTask { get; set; } = true;

    protected override Task<bool> ShouldTaskRunAsync(CancellationToken cancellationToken) => Task.FromResult(IsEnabled);
}