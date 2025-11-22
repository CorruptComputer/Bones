using Bones.Api.Models.Item;
using Bones.Database.DbSets.TaskManagement;

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
    public required int CurrentVersion { get; init; }

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

    /// <summary>
    ///   The values for this
    /// </summary>
    public required IEnumerable<ItemValueDisplayModel> ItemValues { get; init; }

    internal static GetTaskByIdResponse FromInternal(BonesTask task)
    {
        return new()
        {
            TaskId = task.Id,
            ProjectId = task.Item.Project.Id,
            Title = task.Item.Versions.First(v => v.Version == task.Item.CurrentVersion).Title,
            LayoutId = task.Item.ItemLayout.Id,
            TaskQueueName = task.TaskQueue.Name,
            TaskQueueId = task.TaskQueue.Id,
            AddedToQueueDateTime = task.AddedToQueueDateTime,
            ItemId = task.Item.Id,
            LatestItemVersionId = task.Item.Current?.Id ?? throw new(),
            CurrentVersion = task.Item.CurrentVersion,
            FriendlyId = task.Item.FriendlyId,
            CreateDateTime = task.Item.CreateDateTime,
            LatestVersionCreateDateTime = task.Item.Current?.CreateDateTime ?? throw new(),
            ItemValues = task.Item.Current.ItemLayoutVersion.FieldLinks.Select(fl => new ItemValueDisplayModel
            {
                OrderNumber = fl.OrderNumber,
                FieldVersionId = fl.FieldVersion.Id,
                Name = fl.FieldVersion.Name,
                ValueType = fl.FieldVersion.Type,
                IsRequired = fl.FieldVersion.IsRequired,
                CanBeNegative = fl.FieldVersion.CanBeNegative,
                PossibleValues = fl.FieldVersion.PossibleValues?.Select(v => v.Value),
                Value = task.Item.Current.Values.FirstOrDefault(v => v.Field.Id == fl.FieldVersion.Id)?.Value
            })
        };
    }
}
