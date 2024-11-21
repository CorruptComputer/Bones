namespace Bones.Database.Operations.WorkItemManagement.WorkItems.CreateWorkItemVersionDb;

/// <summary>
///     DB Command for creating an ItemVersion.
/// </summary>
/// <param name="WorkItemId">Internal ID of the item</param>
/// <param name="WorkItemLayoutVersionId">Internal ID of the layout version this item is using</param>
/// <param name="Values">The values to use for this work item version</param>
public record CreateWorkItemVersionDbCommand(Guid WorkItemId, Guid WorkItemLayoutVersionId, Dictionary<string, object> Values) : IRequest<CommandResponse>;