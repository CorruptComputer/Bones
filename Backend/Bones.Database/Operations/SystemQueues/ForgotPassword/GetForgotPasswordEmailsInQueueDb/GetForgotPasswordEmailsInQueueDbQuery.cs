using Bones.Database.DbSets.SystemQueues;

namespace Bones.Database.Operations.SystemQueues.ForgotPassword.GetConfirmationEmailsInQueueDb;

/// <summary>
///   Checks if any confirmation emails are in the queue
/// </summary>
public sealed record GetForgotPasswordEmailsInQueueDbQuery : IRequest<QueryResponse<List<ForgotPasswordEmailQueue>>>;