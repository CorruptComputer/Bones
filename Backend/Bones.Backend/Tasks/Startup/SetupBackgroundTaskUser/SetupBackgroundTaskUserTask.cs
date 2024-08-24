namespace Bones.Backend.Tasks.Startup.SetupBackgroundTaskUser;

public class SetupBackgroundTaskUserTask(Logger<SetupBackgroundTaskUserTask> logger) : StartupTaskBase<SetupBackgroundTaskUserTask>(logger)
{
    protected override Task RunTaskAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}