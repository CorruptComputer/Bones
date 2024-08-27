namespace Bones.Backend.Tasks.Minutely.SendConfirmationEmail;

public class SendConfirmationEmailTask(ILogger<SendConfirmationEmailTask> logger, ISender sender) : MinutelyTaskBase<SendConfirmationEmailTask>(logger, sender)
{
    protected override Task<bool> ShouldTaskRunAsync(CancellationToken cancellationToken)
    {
        if (!IsEnabled)
        {
            return Task.FromResult(false);
        }

        // TODO: Check if there are any emails in the queue

        return Task.FromResult(false);
    }

    protected override Task RunTaskAsync(CancellationToken cancellationToken)
    {
        // TODO:
        // Get the emails from the queue
        // While this is in dev, only send to my email address, delete from queue and skip if its not
        // Send the emails in the queue one by one, removing each one from the queue after it is sent successfully
        // If there was an error with sending one, increment the retry count, skip it for now, and continue with the rest
        // If the retry count is greater than 5, move it to the DeadConfirmationEmailQueue

        // By the end of this, all the successfully sent emails should be removed from the DB, just leaving the unsuccessful ones to be retried

        return Task.CompletedTask;
    }
}