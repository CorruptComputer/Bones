using Bones.Api.Models.WorkItems.Actions;
using Bones.Database.DbSets.AccountManagement;

namespace Bones.Api.Models.WorkItems;

/// <summary>
///   An action that can be performed on a work item
/// </summary>
[JsonPolymorphic] // This shit doesn't work with NSwag, need to find an alternative
[JsonDerivedType(typeof(AssignWorkItemAction), nameof(AssignWorkItemAction))]
[JsonDerivedType(typeof(CreateWorkItemAction), nameof(CreateWorkItemAction))]
[JsonDerivedType(typeof(CreateWorkItemVersionAction), nameof(CreateWorkItemVersionAction))]
[JsonDerivedType(typeof(DeleteWorkItemAction), nameof(DeleteWorkItemAction))]
[JsonDerivedType(typeof(DeleteWorkItemVersionAction), nameof(DeleteWorkItemVersionAction))]
[JsonDerivedType(typeof(MoveWorkItemQueueAction), nameof(MoveWorkItemQueueAction))]
[JsonDerivedType(typeof(UnassignWorkItemAction), nameof(UnassignWorkItemAction))]
public abstract record WorkItemActionBase
{
    /// <summary>
    ///   The timestamp of when this action was performed
    /// </summary>
    public required DateTimeOffset ActionDateTime { get; init; }

    internal abstract Task<IRequest<CommandResponse>> ToInternalAsync(BonesUser user, ISender sender);

    internal abstract Task<WorkItemActionResponse> FromInternalAsync(CommandResponse result, BonesUser user, ISender sender);
}
