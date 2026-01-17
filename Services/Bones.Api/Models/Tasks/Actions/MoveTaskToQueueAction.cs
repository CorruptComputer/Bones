using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.TaskManagement;
using Bones.Logic.Features.Tasks.TaskQueues;
using Bones.Logic.Features.Tasks.Tasks;
using Bones.Shared.Exceptions;

namespace Bones.Api.Models.Tasks.Actions;

/// <summary>
///   Action to move a  to a different queue.
/// </summary>
[JsonSerializable(typeof(MoveTaskToQueueAction))]
public sealed record class MoveTaskToQueueAction : TaskActionBase
{
    /// <summary>
    ///   The ID of the  to perform the action on
    /// </summary>
    public required Guid TaskId { get; init; }

    /// <summary>
    ///   The ID of the  queue to move the  to
    /// </summary>
    public required Guid TaskQueueId { get; init; }

    internal override async Task<IRequest<CommandResponse>> ToInternalAsync(BonesUser user, ISender sender)
    {
        return await Task.FromResult(new MoveTaskToQueue.Command(TaskId, TaskQueueId, ActionDateTime, user));
    }

    internal override async Task<TaskActionResponse> FromInternalAsync(CommandResponse result, BonesUser user, ISender sender)
    {
        BonesTask? task = await sender.Send(new GetTaskById.Query(TaskId, user), CancellationToken.None);

        if (task is null)
        {
            throw new BonesException(" not found after moving to queue");
        }

        return new TaskActionResponse
        {
            TaskId = task.Id,
            TaskCurrentVersionId = task.Item!.Current?.Id ?? Guid.Empty,
        };
    }
}
