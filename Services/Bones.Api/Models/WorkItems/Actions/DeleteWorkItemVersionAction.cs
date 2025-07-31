using Bones.Database.DbSets.AccountManagement;

namespace Bones.Api.Models.WorkItems.Actions;

/// <summary>
///   Action to delete a specific version of a work item.
/// </summary>
[JsonSerializable(typeof(DeleteWorkItemVersionAction))]
public sealed record class DeleteWorkItemVersionAction : WorkItemActionBase
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
