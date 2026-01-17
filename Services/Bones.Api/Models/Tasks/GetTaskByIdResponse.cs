using Bones.Api.Models.Item;
using Bones.Database.DbSets.Items;
using Bones.Database.DbSets.TaskManagement;
using Bones.Shared.Exceptions;

namespace Bones.Api.Models.Tasks;

/// <summary>
///   Response for the GetTaskById endpoint
/// </summary>
[JsonSerializable(typeof(GetTaskByIdResponse))]
public sealed record GetTaskByIdResponse
{
    /// <summary>
    ///   ID of the
    /// </summary>
    public required Guid TaskId { get; init; }

    /// <summary>
    ///   The title of the
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    ///   The ID of the layout this  uses
    /// </summary>
    public required Guid LayoutId { get; init; }

    /// <summary>
    ///   The ID of the project this  belongs to
    /// </summary>
    public required Guid ProjectId { get; init; }

    /// <summary>
    ///   ID of the  queue this item belongs to
    /// </summary>
    public required Guid TaskQueueId { get; init; }

    /// <summary>
    ///   The name of the queue this item is in
    /// </summary>
    public required string TaskQueueName { get; init; }

    /// <summary>
    ///   The time this item was added to the queue
    /// </summary>
    public required DateTimeOffset AddedToQueueDateTime { get; init; }

    /// <summary>
    ///   The ID of the item this  is
    /// </summary>
    public required Guid ItemId { get; init; }

    /// <summary>
    ///   The ID of the latest version of the item this  is
    /// </summary>
    public required Guid LatestItemVersionId { get; init; }

    /// <summary>
    ///   The current version number for this
    /// </summary>
    public required long CurrentVersion { get; init; }

    /// <summary>
    ///   The friendly ID of the
    /// </summary>
    public required string FriendlyId { get; init; }

    /// <summary>
    ///   The time this item was created
    /// </summary>
    public required DateTimeOffset CreateDateTime { get; init; }

    /// <summary>
    ///   The time the latest version of this item was created
    /// </summary>
    public required DateTimeOffset LatestVersionCreateDateTime { get; init; }

    internal static GetTaskByIdResponse FromInternal(BonesTask task)
    {
        ItemVersion? currentVersion = task.Item!.Current;
        if (currentVersion is null)
        {
            throw new BonesException("Current item version is null");
        }

        ItemLayoutVersion? itemLayoutVersion = currentVersion.ItemLayoutVersion;
        if (itemLayoutVersion is null)
        {
            throw new BonesException("Current item layout version is null");
        }

        return new()
        {
            TaskId = task.Id,
            ProjectId = task.Item.ProjectId,
            Title = currentVersion.Title,
            LayoutId = task.Item.ItemLayoutId,
            TaskQueueName = task.TaskQueue!.Name,
            TaskQueueId = task.TaskQueueId,
            AddedToQueueDateTime = task.AddedToQueueDateTime,
            ItemId = task.Item.Id,
            LatestItemVersionId = currentVersion.Id,
            CurrentVersion = task.Item.CurrentVersion,
            FriendlyId = task.Item.FriendlyId,
            CreateDateTime = task.Item.CreateDateTime,
            LatestVersionCreateDateTime = currentVersion.CreateDateTime
        };
    }
}
