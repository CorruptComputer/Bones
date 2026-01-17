using Bones.Database.DbSets.TaskManagement;

namespace Bones.Api.Models.TaskQueues;

/// <summary>
///   Response for the GetTaskQueueDashboardAsync endpoint
/// </summary>
[JsonSerializable(typeof(GetTaskQueueDashboardResponse))]
public sealed record GetTaskQueueDashboardResponse
{
    /// <summary>
    ///   The name of the queue
    /// </summary>
    public required string QueueName { get; init; }

    /// <summary>
    ///   The s in this queue, ordered by date added from oldest to newest
    /// </summary>
    public required List<DashboardTaskModel> Tasks { get; init; }


    internal static GetTaskQueueDashboardResponse FromInternal(TaskQueue queue)
    {
        return new()
        {
            QueueName = queue.Name,
            Tasks = [.. queue.BonesTasks.Select(DashboardTaskModel.FromTask)]
        };
    }

    /// <summary>
    ///   Model for a Task in the dashboard
    /// </summary>
    public record DashboardTaskModel
    {
        /// <summary>
        ///   The ID of the Task
        /// </summary>
        public required Guid Id { get; init; }

        /// <summary>
        ///   The friendly ID of the Task
        /// </summary>
        public required string FriendlyId { get; init; }

        /// <summary>
        ///   The title of the Task
        /// </summary>
        public required string Title { get; init; }

        /// <summary>
        ///   The date and time the Task was added to the queue
        /// </summary>
        public required DateTimeOffset AddedToQueueDateTime { get; init; }

        internal static DashboardTaskModel FromTask(BonesTask task)
        {
            return new()
            {
                Id = task.Id,
                FriendlyId = task.Item!.FriendlyId,
                Title = task.Item.Current?.Title ?? string.Empty,
                AddedToQueueDateTime = task.AddedToQueueDateTime
            };
        }
    }
}
