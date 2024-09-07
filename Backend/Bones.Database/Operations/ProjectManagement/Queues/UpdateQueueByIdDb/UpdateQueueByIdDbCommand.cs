namespace Bones.Database.Operations.ProjectManagement.Queues.UpdateQueueByIdDb;

/// <summary>
///     DB Command for updating a Queue.
/// </summary>
/// <param name="QueueId">Internal ID of the queue</param>
/// <param name="NewName">The new name of the queue</param>
public sealed record UpdateQueueByIdDbCommand(Guid QueueId, string NewName) : IRequest<CommandResponse>;