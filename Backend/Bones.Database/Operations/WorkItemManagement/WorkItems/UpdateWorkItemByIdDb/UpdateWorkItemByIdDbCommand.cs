namespace Bones.Database.Operations.WorkItemManagement.WorkItems.UpdateWorkItemByIdDb;

/// <summary>
///     DB Command for updating an WorkItem.
/// </summary>
/// <param name="WorkItemId">Internal ID of the item</param>
/// <param name="Name">The new name of the item</param>
/// <param name="QueueId">Internal ID of the queue</param>
public record UpdateWorkItemByIdDbCommand(Guid WorkItemId, string Name, Guid QueueId) : IRequest<CommandResponse>;