namespace Bones.Database.Operations.WorkItemManagement.WorkItems.CreateWorkItemDb;

/// <summary>
///     DB Command for creating an WorkItem.
/// </summary>
/// <param name="Name">Name of the item</param>
/// <param name="QueueId">Internal ID of the queue this item is in</param>
/// <param name="ItemLayoutId">Internal ID of the layout this item is using</param>
public sealed record CreateWorkItemDbCommand(string Name, Guid QueueId, Guid ItemLayoutId) : IRequest<CommandResponse>;