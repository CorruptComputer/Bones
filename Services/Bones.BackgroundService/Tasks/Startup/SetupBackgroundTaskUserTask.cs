namespace Bones.BackgroundService.Tasks.Startup;

internal class SetupBackgroundTaskUserTask(ISender sender) : StartupTaskBase(sender)
{
    protected override Task RunTaskAsync(CancellationToken cancellationToken)
    {
        // TODO:
        // Get the email for the background task user
        // Get the optional userId for if you want to change the email after
        // If the user id is set, get that user
        //   if the users email matches whats configured, do nothing
        //   if the email doesn't match, update it
        // If its not set, check if any users in the DB have the configured email
        //   If none exist with that email, create the user
        //   If it exists, do nothing

        return Task.CompletedTask;
    }
}