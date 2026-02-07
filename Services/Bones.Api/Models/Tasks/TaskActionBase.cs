using Bones.Api.Models.Tasks.Actions;
using Bones.Database.DbSets.Accounts;

namespace Bones.Api.Models.Tasks;

/// <summary>
///   An action that can be performed on a
/// </summary>
[JsonPolymorphic] // This shit doesn't work with NSwag, need to find an alternative
[JsonDerivedType(typeof(AssignTaskAction), nameof(AssignTaskAction))]
[JsonDerivedType(typeof(CreateTaskAction), nameof(CreateTaskAction))]
[JsonDerivedType(typeof(CreateTaskVersionAction), nameof(CreateTaskVersionAction))]
[JsonDerivedType(typeof(DeleteTaskAction), nameof(DeleteTaskAction))]
[JsonDerivedType(typeof(DeleteTaskVersionAction), nameof(DeleteTaskVersionAction))]
[JsonDerivedType(typeof(MoveTaskToQueueAction), nameof(MoveTaskToQueueAction))]
[JsonDerivedType(typeof(UnassignTaskAction), nameof(UnassignTaskAction))]
public abstract record TaskActionBase
{
    /// <summary>
    ///   The timestamp of when this action was performed
    /// </summary>
    public required DateTimeOffset ActionDateTime { get; init; }

    internal abstract Task<IRequest<CommandResponse>> ToInternalAsync(BonesUser user, ISender sender);

    internal abstract Task<TaskActionResponse> FromInternalAsync(CommandResponse result, BonesUser user, ISender sender);
}
