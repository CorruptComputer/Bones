using Bones.Database.DbSets.System;

namespace Bones.Database.Operations.SystemQueues.ForgotPassword.GetForgotPasswordEmailsInQueueDb;

/// <summary>
///   Checks if any confirmation emails are in the queue
/// </summary>
public sealed record GetForgotPasswordEmailsInQueueDbQuery : IRequest<QueryResponse<List<ForgotPasswordEmailQueue>>>;