namespace Bones.BackgroundService.Tasks.Startup;

internal class SetupBackgroundTaskUserTask(Logger<SetupBackgroundTaskUserTask> logger, ISender sender) : StartupTaskBase<SetupBackgroundTaskUserTask>(logger, sender)
{
    protected override Task RunTaskAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}