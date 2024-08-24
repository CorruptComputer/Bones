namespace Bones.Backend.Tasks.Minutely.SendConfirmationEmail;

public class SendConfirmationEmailTask(ILogger<SendConfirmationEmailTask> logger) : MinutelyTaskBase<SendConfirmationEmailTask>(logger)
{
    protected override Task RunTaskAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}