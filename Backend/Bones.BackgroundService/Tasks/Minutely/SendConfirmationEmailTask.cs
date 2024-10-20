using System.Net;
using System.Net.Mail;
using Bones.BackgroundService.Models;
using Bones.Database.DbSets.SystemQueues;
using Bones.Database.Operations.SystemQueues.ConfirmationEmail.AnyConfirmationEmailsInQueueDb;
using Bones.Database.Operations.SystemQueues.ConfirmationEmail.GetConfirmationEmailsInQueueDb;
using Bones.Database.Operations.SystemQueues.ConfirmationEmail.IncrementFailedConfirmationEmailById;
using Bones.Database.Operations.SystemQueues.ConfirmationEmail.RemoveConfirmationEmailFromQueueByIdDb;

namespace Bones.BackgroundService.Tasks.Minutely;

internal class SendConfirmationEmailTask(ISender sender, BackgroundServiceConfiguration configuration) : MinutelyTaskBase(sender)
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

        return await Sender.Send(new AnyConfirmationEmailsInQueueDbQuery(), cancellationToken);
    }

    protected override async Task RunTaskAsync(CancellationToken cancellationToken)
    {
        List<ConfirmationEmailQueue>? emailsInQueue = await Sender.Send(new GetConfirmationEmailsInQueueDbQuery(), cancellationToken);

        if (emailsInQueue is null)
        {
            return;
        }

        Log.Information("{Count} confirmation emails in queue, ready to send.", emailsInQueue.Count);

        using SmtpClient client = new(configuration.SmtpServer, configuration.SmtpPort ?? 25);
        foreach (ConfirmationEmailQueue emailToSend in emailsInQueue)
        {
            try
            {
                MailMessage message = new(
                    configuration.BackgroundServiceUserEmail!,
                    emailToSend.EmailTo,
                    "Confirmation Email",
                    emailToSend.ConfirmationLink);

                client.Credentials = new NetworkCredential(configuration.SmtpUser, configuration.SmtpPassword);
                client.Send(message);

                await Sender.Send(new RemoveConfirmationEmailFromQueueByIdDbCommand(emailToSend.Id), cancellationToken);
            }
            catch (Exception ex)
            {
                await Sender.Send(new IncrementFailedConfirmationEmailByIdCommand(emailToSend.Id, ex.Message), cancellationToken);
            }
        }
    }
}