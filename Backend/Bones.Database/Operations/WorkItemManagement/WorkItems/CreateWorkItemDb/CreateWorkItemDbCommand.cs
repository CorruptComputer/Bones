namespace Bones.Database.Operations.WorkItemManagement.WorkItems.CreateWorkItemDb;

/// <summary>
///     DB Command for creating an WorkItem.
/// </summary>
/// <param name="Name">Name of the item</param>
/// <param name="QueueId">Internal ID of the queue this item is in</param>
/// <param name="WorkItemLayoutVersionId">Internal ID of the layout version this item is using</param>
/// <param name="Values">Internal ID of the queue</param>
public sealed record CreateWorkItemDbCommand(string Name, Guid QueueId, Guid WorkItemLayoutVersionId, Dictionary<string, object> Values)
    : IRequest<CommandResponse>;