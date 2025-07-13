using Bones.Database.DbSets.AccountManagement;

namespace Bones.Api.Models.WorkItems.Actions;

/// <summary>
///   An action that can be performed on a work item
/// </summary>
[JsonPolymorphic]
[JsonDerivedType(typeof(AssignWorkItemAction), nameof(AssignWorkItemAction))]
[JsonDerivedType(typeof(CreateWorkItemAction), nameof(CreateWorkItemAction))]
[JsonDerivedType(typeof(CreateWorkItemVersionAction), nameof(CreateWorkItemVersionAction))]
[JsonDerivedType(typeof(DeleteWorkItemAction), nameof(DeleteWorkItemAction))]
[JsonDerivedType(typeof(DeleteWorkItemVersionAction), nameof(DeleteWorkItemVersionAction))]
[JsonDerivedType(typeof(MoveWorkItemQueueAction), nameof(MoveWorkItemQueueAction))]
[JsonDerivedType(typeof(UnassignWorkItemAction), nameof(UnassignWorkItemAction))]
public abstract class WorkItemActionBase
{
    /// <summary>
    ///   The timestamp of when this action was performed
    /// </summary>
    public required DateTimeOffset ActionDateTime { get; init; }

    internal abstract void ToInternal(BonesUser user);
}
