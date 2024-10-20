namespace Bones.Database.Operations.WorkItemManagement.WorkItems.QueueDeleteWorkItemVersionByIdDb;

/// <summary>
///     DB Command for deleting an ItemVersion.
/// </summary>
/// <param name="WorkItemVersionId">Internal ID of the item version</param>
public record QueueDeleteWorkItemVersionByIdDbCommand(Guid WorkItemVersionId) : IRequest<CommandResponse>;