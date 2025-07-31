using Bones.Database.DbSets.WorkItemManagement;

namespace Bones.Api.Models.WorkItems;

/// <summary>
///   Response for the GetWorkItemQueueDashboardAsync endpoint
/// </summary>
[JsonSerializable(typeof(GetWorkItemQueueDashboardResponse))]
public sealed record GetWorkItemQueueDashboardResponse
{
    /// <summary>
    ///   The name of the queue
    /// </summary>
    public required string QueueName { get; init; }

    /// <summary>
    ///   The work items in this queue, ordered by date added from oldest to newest
    /// </summary>
    public required List<DashboardWorkItemModel> WorkItems { get; init; }


    internal static GetWorkItemQueueDashboardResponse FromInternal(WorkItemQueue queue)
    {
        return new()
        {
            QueueName = queue.Name,
            WorkItems = [.. queue.WorkItems.Select(DashboardWorkItemModel.FromWorkItem)]
        };
    }

    /// <summary>
    ///   Model for a work item in the dashboard
    /// </summary>
    public record DashboardWorkItemModel
    {
        /// <summary>
        ///   The ID of the work item
        /// </summary>
        public required Guid Id { get; init; }

        /// <summary>
        ///   The friendly ID of the work item
        /// </summary>
        public required string FriendlyId { get; init; }

        /// <summary>
        ///   The title of the work item
        /// </summary>
        public required string Title { get; init; }

        /// <summary>
        ///   The date and time the work item was added to the queue
        /// </summary>
        public required DateTimeOffset AddedToQueueDateTime { get; init; }

        internal static DashboardWorkItemModel FromWorkItem(WorkItem workItem)
        {
            return new()
            {
                Id = workItem.Id,
                FriendlyId = workItem.Item.FriendlyId,
                Title = workItem.CurrentVersion?.Title ?? string.Empty,
                AddedToQueueDateTime = workItem.AddedToQueueDateTime
            };
        }
    }
}
