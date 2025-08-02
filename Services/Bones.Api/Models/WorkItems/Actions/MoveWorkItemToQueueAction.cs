using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.WorkItemManagement;
using Bones.Logic.Features.WorkItems.Queue;
using Bones.Logic.Features.WorkItems.WorkItems;
using Bones.Shared.Exceptions;

namespace Bones.Api.Models.WorkItems.Actions;

/// <summary>
///   Action to move a work item to a different queue.
/// </summary>
[JsonSerializable(typeof(MoveWorkItemToQueueAction))]
public sealed record class MoveWorkItemToQueueAction : WorkItemActionBase
{
    /// <summary>
    ///   The ID of the work item to perform the action on
    /// </summary>
    public required Guid WorkItemId { get; init; }

    /// <summary>
    ///   The ID of the work item queue to move the work item to
    /// </summary>
    public required Guid WorkItemQueueId { get; init; }

    internal override async Task<IRequest<CommandResponse>> ToInternalAsync(BonesUser user, ISender sender)
    {
        return await Task.FromResult(new MoveWorkItemToQueue.Command(WorkItemId, WorkItemQueueId, ActionDateTime, user));
    }

    internal override async Task<WorkItemActionResponse> FromInternalAsync(CommandResponse result, BonesUser user, ISender sender)
    {
        WorkItem? workItem = await sender.Send(new GetWorkItemById.Query(WorkItemId, user), CancellationToken.None);

        if (workItem is null)
        {
            throw new BonesException("Work item not found after moving to queue");
        }

        return new WorkItemActionResponse
        {
            WorkItemId = workItem.Id,
            WorkItemCurrentVersionId = workItem.CurrentVersion?.Id ?? Guid.Empty,
        };
    }
}
