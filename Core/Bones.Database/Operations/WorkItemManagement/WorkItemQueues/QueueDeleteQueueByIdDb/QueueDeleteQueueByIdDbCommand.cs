namespace Bones.Database.Operations.WorkItemManagement.WorkItemQueues.QueueDeleteQueueByIdDb;

/// <summary>
///     DB Command for deleting a Queue.
/// </summary>
/// <param name="QueueId">Internal ID of the queue</param>
public sealed record QueueDeleteQueueByIdDbCommand(Guid QueueId) : IRequest<CommandResponse>;