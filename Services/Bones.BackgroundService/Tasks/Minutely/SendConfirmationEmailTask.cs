using System.Net;
using System.Net.Mail;
using Bones.BackgroundService.Models;
using Bones.Database.DbSets.System;
using Bones.Database.Operations.System;

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

        return await Sender.Send(new AnyConfirmationEmailsInQueueDb.Query(), cancellationToken);
    }

    protected override async Task RunTaskAsync(CancellationToken cancellationToken)
    {
        List<ConfirmationEmailQueue>? emailsInQueue = await Sender.Send(new GetConfirmationEmailsInQueueDb.Query(), cancellationToken);

        if (emailsInQueue is null)
        {
            return;
        }

        Log.Information("{Count} confirmation emails in queue, ready to send.", emailsInQueue.Count);

        using SmtpClient client = new(configuration.SmtpServer, configuration.SmtpPort ?? 25);
        client.EnableSsl = true;
        client.Credentials = new NetworkCredential(configuration.SmtpUser, configuration.SmtpPassword);

        foreach (ConfirmationEmailQueue emailToSend in emailsInQueue)
        {
            try
            {
                MailMessage message = new(
                    configuration.BackgroundServiceUserEmail!,
                    emailToSend.EmailTo,
                    "Confirmation Email",
                    emailToSend.ConfirmationLink);


                client.Send(message);

                await Sender.Send(new RemoveConfirmationEmailFromQueueByIdDb.Command(emailToSend.Id), cancellationToken);
            }
            catch (Exception ex)
            {
                await Sender.Send(new IncrementFailedConfirmationEmailById.Command(emailToSend.Id, ex.Message), cancellationToken);
            }
        }
    }
}