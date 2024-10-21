using System.Net.Mail;
using Bones.BackgroundService.Models;
using Bones.Database.DbSets.SystemQueues;
using Bones.Database.Operations.SystemQueues.ConfirmationEmail.GetConfirmationEmailsInQueueDb;
using Bones.Database.Operations.SystemQueues.ConfirmationEmail.IncrementFailedConfirmationEmailById;
using Bones.Database.Operations.SystemQueues.ConfirmationEmail.RemoveConfirmationEmailFromQueueByIdDb;
using Bones.Database.Operations.SystemQueues.ForgotPassword.AnyForgotPasswordEmailsInQueueDb;
using Bones.Database.Operations.SystemQueues.ForgotPassword.GetConfirmationEmailsInQueueDb;
using Bones.Database.Operations.SystemQueues.ForgotPassword.IncrementFailedConfirmationEmailById;
using Bones.Database.Operations.SystemQueues.ForgotPassword.RemoveConfirmationEmailFromQueueByIdDb;

namespace Bones.BackgroundService.Tasks.Minutely;

internal class SendForgotPasswordEmailTask(ISender sender, BackgroundServiceConfiguration configuration) : MinutelyTaskBase(sender)
{
    protected override async Task<bool> ShouldTaskRunAsync(CancellationToken cancellationToken)
    {
        if (!IsEnabled)
        {
            return false;
        }

        if (configuration.BackgroundServiceUserEmail is null || configuration.SmtpServer is null || configuration.SmtpPort is null)
        {
            Log.Warning("BackgroundServiceConfiguration:BackgroundServiceUserEmail or BackgroundServiceConfiguration:SmtpServer or BackgroundServiceConfiguration:SmtpPort is null.");
            IsEnabled = false;
            return false;
        }

        return await Sender.Send(new AnyForgotPasswordEmailsInQueueDbQuery(), cancellationToken);
    }

    protected override async Task RunTaskAsync(CancellationToken cancellationToken)
    {
        List<ForgotPasswordEmailQueue>? emailsInQueue = await Sender.Send(new GetForgotPasswordEmailsInQueueDbQuery(), cancellationToken);

        if (emailsInQueue is null)
        {
            return;
        }

        Log.Information("{Count} confirmation emails in queue, ready to send.", emailsInQueue.Count);

        using SmtpClient client = new(configuration.SmtpServer, configuration.SmtpPort ?? 25);
        foreach (ForgotPasswordEmailQueue emailToSend in emailsInQueue)
        {
            try
            {
                MailMessage message = new(
                    configuration.BackgroundServiceUserEmail!,
                    emailToSend.EmailTo,
                    "Password Reset Email",
                    emailToSend.PasswordResetLink);

                client.Send(message);

                await Sender.Send(new RemoveForgotPasswordEmailFromQueueByIdDbCommand(emailToSend.Id), cancellationToken);
            }
            catch (Exception ex)
            {
                await Sender.Send(new IncrementFailedForgotPasswordEmailByIdCommand(emailToSend.Id, ex.Message), cancellationToken);
            }
        }
    }
}