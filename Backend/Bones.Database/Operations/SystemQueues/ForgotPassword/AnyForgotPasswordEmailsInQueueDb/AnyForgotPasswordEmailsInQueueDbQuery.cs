namespace Bones.Database.Operations.SystemQueues.ForgotPassword.AnyForgotPasswordEmailsInQueueDb;

/// <summary>
///   Checks if any confirmation emails are in the queue
/// </summary>
public sealed record AnyForgotPasswordEmailsInQueueDbQuery : IRequest<QueryResponse<bool>>;