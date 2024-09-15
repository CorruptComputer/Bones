namespace Bones.Database.Operations.SystemQueues.AddConfirmationEmailToQueueDb;

/// <summary>
///   Adds an email confirmation to the queue
/// </summary>
/// <param name="EmailTo">The email being confirmed and the email will be sent to</param>
/// <param name="ConfirmationLink">The link for them to click</param>
public sealed record AddConfirmationEmailToQueueDbCommand(string EmailTo, string ConfirmationLink) : IRequest<CommandResponse>;