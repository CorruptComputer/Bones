namespace Bones.Backend.Features.ProjectManagement.WorkItems.CreateWorkItemVersion;

/// <summary>
///     DB Command for creating a WorkItem.
/// </summary>
/// <param name="WorkItemId">Internal ID of the queue this item is in</param>
/// <param name="WorkItemLayoutVersionId">Internal ID of the layout version this item is using</param>
/// <param name="Values">Internal ID of the queue</param>
public sealed record CreateWorkItemVersionCommand(Guid WorkItemId, Guid WorkItemLayoutVersionId, Dictionary<string, object> Values) : IRequest<CommandResponse>;