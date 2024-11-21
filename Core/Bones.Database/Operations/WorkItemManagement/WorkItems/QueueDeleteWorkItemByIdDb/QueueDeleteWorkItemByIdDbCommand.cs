namespace Bones.Database.Operations.WorkItemManagement.WorkItems.QueueDeleteWorkItemByIdDb;

/// <summary>
///     DB Command for deleting an WorkItem.
/// </summary>
/// <param name="WorkItemId">Internal ID of the item</param>
public record QueueDeleteWorkItemByIdDbCommand(Guid WorkItemId) : IRequest<CommandResponse>;