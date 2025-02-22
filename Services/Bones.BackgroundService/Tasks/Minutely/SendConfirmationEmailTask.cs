using System.Net;
using System.Net.Mail;
using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.System;
using Bones.Database.Operations.System.Queues;
using Bones.Database.Operations.System.SystemSettings;
using Bones.Database.Operations.System.SystemSettings.Models;

namespace Bones.BackgroundService.Tasks.Minutely;

internal class SendConfirmationEmailTask(ISender sender) : MinutelyTaskBase(sender)
{
    protected override async Task<bool> ShouldTaskRunAsync(CancellationToken cancellationToken)
    {
        if (!IsEnabled)
        {
            return false;
        }

        BonesUser? backgroundServiceUser = await Sender.Send(new GetBackgroundServiceUserDb.Query(), cancellationToken);
        if (backgroundServiceUser is null)
        {
            // Most likely this thread won the race at startup for all the background tasks on first run
            Log.Warning("Background service user not found.");
            return false;
        }

        SmtpConfig? smtpConfig = await Sender.Send(new GetSmtpConfigDb.Query(), cancellationToken);

        if (smtpConfig?.Server is null || smtpConfig.Port is null)
        {
            Log.Warning("SMTP not configured.");
            return false;
        }

        return await Sender.Send(new AnyConfirmationEmailsInQueueDb.Query(), cancellationToken);
    }

    protected override async Task RunTaskAsync(CancellationToken cancellationToken)
    {
        List<ConfirmationEmailQueue>? emailsInQueue = await Sender.Send(new GetConfirmationEmailsInQueueDb.Query(), cancellationToken);
        BonesUser? backgroundServiceUser = await Sender.Send(new GetBackgroundServiceUserDb.Query(), cancellationToken);
        SmtpConfig? smtpConfig = await Sender.Send(new GetSmtpConfigDb.Query(), cancellationToken);

        if (emailsInQueue is null || backgroundServiceUser is null || smtpConfig?.Server is null || smtpConfig.Port is null || smtpConfig.UseSsl is null)
        {
            return;
        }

        using SmtpClient client = new(smtpConfig.Server, smtpConfig.Port.Value);
        client.EnableSsl = smtpConfig.UseSsl.Value;
        if (smtpConfig.Username is not null)
        {
            client.Credentials = new NetworkCredential(smtpConfig.Username, smtpConfig.Password);
        }

        string? emailFrom = smtpConfig.FromAddress ?? backgroundServiceUser.Email;

        // Shouldn't really be possible, but to get the warning out of the way
        if (emailFrom is null)
        {
            Log.Warning("No From Address configured for SMTP.");
            return;
        }

        Log.Information("{Count} confirmation emails in queue, ready to send.", emailsInQueue.Count);

        foreach (ConfirmationEmailQueue emailToSend in emailsInQueue)
        {
            try
            {
                MailAddress fromAddress = new(emailFrom, smtpConfig.FromName ?? backgroundServiceUser.Email);
                MailAddress toAddress = new(emailToSend.EmailTo);

                MailMessage message = new(fromAddress, toAddress)
                {
                    Body = emailToSend.ConfirmationLink,
                    Subject = "Confirmation Email"
                };

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