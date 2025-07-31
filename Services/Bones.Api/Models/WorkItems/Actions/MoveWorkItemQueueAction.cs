using Bones.Database.DbSets.AccountManagement;

namespace Bones.Api.Models.WorkItems.Actions;

/// <summary>
///   Action to move a work item to a different queue.
/// </summary>
[JsonSerializable(typeof(MoveWorkItemQueueAction))]
public sealed record class MoveWorkItemQueueAction : WorkItemActionBase
{
    /// <summary>
    ///   The ID of the work item to perform the action on
    /// </summary>
    public required Guid WorkItemId { get; init; }

    internal override Task<IRequest<CommandResponse>> ToInternalAsync(BonesUser user, ISender sender)
    {
        throw new NotImplementedException();
    }

    internal override Task<WorkItemActionResponse> FromInternalAsync(CommandResponse result, BonesUser user, ISender sender)
    {
        throw new NotImplementedException();
    }
}
