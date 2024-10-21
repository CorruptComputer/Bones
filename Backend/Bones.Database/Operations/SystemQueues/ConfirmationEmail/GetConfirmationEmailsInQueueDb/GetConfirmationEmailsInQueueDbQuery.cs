using Bones.Database.DbSets.SystemQueues;

namespace Bones.Database.Operations.SystemQueues.ConfirmationEmail.GetConfirmationEmailsInQueueDb;

/// <summary>
///   Checks if any confirmation emails are in the queue
/// </summary>
public sealed record GetConfirmationEmailsInQueueDbQuery : IRequest<QueryResponse<List<ConfirmationEmailQueue>>>;