using Bones.Api.Models.Item;
using Bones.Database.DbSets.WorkItemManagement;

namespace Bones.Api.Models.WorkItems;

/// <summary>
///   Response for the GetWorkItemById endpoint
/// </summary>
[JsonSerializable(typeof(GetWorkItemByIdResponse))]
public sealed record GetWorkItemByIdResponse
{
    /// <summary>
    ///   ID of the work item
    /// </summary>
    public required Guid WorkItemId { get; init; }

    /// <summary>
    ///   The title of the work item
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    ///   The ID of the layout this work item uses
    /// </summary>
    public required Guid LayoutId { get; init; }

    /// <summary>
    ///   The ID of the project this work item belongs to
    /// </summary>
    public required Guid ProjectId { get; init; }

    /// <summary>
    ///   ID of the work item queue this item belongs to
    /// </summary>
    public required Guid WorkItemQueueId { get; init; }

    /// <summary>
    ///   The name of the queue this item is in
    /// </summary>
    public required string WorkItemQueueName { get; init; }

    /// <summary>
    ///   The time this item was added to the queue
    /// </summary>
    public required DateTimeOffset AddedToQueueDateTime { get; init; }

    /// <summary>
    ///   The ID of the  item this work item is
    /// </summary>
    public required Guid ItemId { get; init; }

    /// <summary>
    ///   The ID of the latest version of the  item this work item is
    /// </summary>
    public required Guid LatestItemVersionId { get; init; }

    /// <summary>
    ///   The current version number for this work item
    /// </summary>
    public required int CurrentVersion { get; init; }

    /// <summary>
    ///   The friendly ID of the work item
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
    ///   The values for this work item
    /// </summary>
    public required IEnumerable<ItemValueDisplayModel> ItemValues { get; init; }

    internal static GetWorkItemByIdResponse FromInternal(WorkItem workItem)
    {
        return new()
        {
            WorkItemId = workItem.Id,
            ProjectId = workItem.Item.Project.Id,
            Title = workItem.Item.Versions.First(v => v.Version == workItem.Item.CurrentVersion).Title,
            LayoutId = workItem.Item.ItemLayout.Id,
            WorkItemQueueName = workItem.WorkItemQueue.Name,
            WorkItemQueueId = workItem.WorkItemQueue.Id,
            AddedToQueueDateTime = workItem.AddedToQueueDateTime,
            ItemId = workItem.Item.Id,
            LatestItemVersionId = workItem.CurrentVersion?.Id ?? throw new(),
            CurrentVersion = workItem.Item.CurrentVersion,
            FriendlyId = workItem.Item.FriendlyId,
            CreateDateTime = workItem.Item.CreateDateTime,
            LatestVersionCreateDateTime = workItem.CurrentVersion?.CreateDateTime ?? throw new(),
            ItemValues = workItem.CurrentVersion.ItemLayoutVersion.FieldLinks.Select(fl => new ItemValueDisplayModel
            {
                OrderNumber = fl.OrderNumber,
                FieldVersionId = fl.FieldVersion.Id,
                Name = fl.FieldVersion.Name,
                ValueType = fl.FieldVersion.Type,
                IsRequired = fl.FieldVersion.IsRequired,
                CanBeNegative = fl.FieldVersion.CanBeNegative,
                PossibleValues = fl.FieldVersion.PossibleValues?.Select(v => v.Value),
                Value = workItem.CurrentVersion.Values.FirstOrDefault(v => v.Field.Id == fl.FieldVersion.Id)?.Value
            })
        };
    }
}
