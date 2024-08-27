namespace Bones.Backend.Tasks.Startup.SetupBackgroundTaskUser;

public class SetupBackgroundTaskUserTask(Logger<SetupBackgroundTaskUserTask> logger, ISender sender) : StartupTaskBase<SetupBackgroundTaskUserTask>(logger, sender)
{
    protected override Task RunTaskAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}