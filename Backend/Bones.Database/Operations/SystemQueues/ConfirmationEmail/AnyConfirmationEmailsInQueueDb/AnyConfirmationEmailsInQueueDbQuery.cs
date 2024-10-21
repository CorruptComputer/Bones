namespace Bones.Database.Operations.SystemQueues.ConfirmationEmail.AnyConfirmationEmailsInQueueDb;

/// <summary>
///   Checks if any confirmation emails are in the queue
/// </summary>
public sealed record AnyConfirmationEmailsInQueueDbQuery : IRequest<QueryResponse<bool>>;