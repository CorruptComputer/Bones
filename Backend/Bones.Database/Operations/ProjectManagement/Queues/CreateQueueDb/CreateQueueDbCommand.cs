namespace Bones.Database.Operations.ProjectManagement.Queues.CreateQueueDb;

/// <summary>
///     DB Command for creating a Queue.
/// </summary>
/// <param name="Name">Name of the queue</param>
/// <param name="InitiativeId">Internal ID of the initiative</param>
public sealed record CreateQueueDbCommand(string Name, Guid InitiativeId) : IRequest<CommandResponse>;