using Bones.Database.DbSets.Accounts;

namespace Bones.Api.Models.Tasks.Actions;

/// <summary>
///   Action to delete a specific version of a .
/// </summary>
[JsonSerializable(typeof(DeleteTaskVersionAction))]
public sealed record class DeleteTaskVersionAction : TaskActionBase
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
