using Bones.Database.DbSets.AccountManagement;

namespace Bones.Api.Models.Tasks.Actions;

/// <summary>
///   Action to unassign a  from a user.
/// </summary>
[JsonSerializable(typeof(UnassignTaskAction))]
public sealed record class UnassignTaskAction : TaskActionBase
{
    /// <summary>
    ///   The ID of the  to perform the action on
    /// </summary>
    public required Guid TaskId { get; init; }

    internal override Task<IRequest<CommandResponse>> ToInternalAsync(BonesUser user, ISender sender)
    {
        throw new NotImplementedException();
    }

    internal override Task<TaskActionResponse> FromInternalAsync(CommandResponse result, BonesUser user, ISender sender)
    {
        throw new NotImplementedException();
    }
}
