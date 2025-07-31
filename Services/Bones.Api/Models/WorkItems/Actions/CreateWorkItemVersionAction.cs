using Bones.Database.DbSets.AccountManagement;

namespace Bones.Api.Models.WorkItems.Actions;

/// <summary>
///   Action to create a new version of a work item.
/// </summary>
[JsonSerializable(typeof(CreateWorkItemVersionAction))]
public sealed record class CreateWorkItemVersionAction : WorkItemActionBase
{
    /// <summary>
    ///   The ID of the work item to perform the action on
    /// </summary>
    public required Guid WorkItemId { get; init; }

    /// <summary>
    ///   The title of the work item
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    ///   The fields of the work item
    /// </summary>
    public required List<ItemValueModel> FieldValues { get; init; }

    internal override Task<IRequest<CommandResponse>> ToInternalAsync(BonesUser user, ISender sender)
    {
        throw new NotImplementedException();
    }

    internal override Task<WorkItemActionResponse> FromInternalAsync(CommandResponse result, BonesUser user, ISender sender)
    {
        throw new NotImplementedException();
    }
}
