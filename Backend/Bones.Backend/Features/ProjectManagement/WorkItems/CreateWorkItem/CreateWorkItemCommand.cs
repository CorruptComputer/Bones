namespace Bones.Backend.Features.ProjectManagement.WorkItems.CreateWorkItem;

/// <summary>
///     DB Command for creating a WorkItem.
/// </summary>
/// <param name="Name">Name of the item</param>
/// <param name="QueueId">Internal ID of the queue this item is in</param>
/// <param name="ItemLayoutId">Internal ID of the layout this item is using</param>
public sealed record CreateWorkItemCommand(string Name, Guid QueueId, Guid ItemLayoutId) : IRequest<CommandResponse>;