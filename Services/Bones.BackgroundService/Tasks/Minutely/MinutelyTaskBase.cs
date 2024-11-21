namespace Bones.BackgroundService.Tasks.Minutely;

internal abstract class MinutelyTaskBase(ISender sender) : TaskBase(sender)
{
    protected override TimeSpan Interval { get; } = TimeSpan.FromMinutes(1);

    protected override bool IsStartupOnlyTask { get; set; } = false;

    protected override Task<bool> ShouldTaskRunAsync(CancellationToken cancellationToken) => Task.FromResult(IsEnabled);
}