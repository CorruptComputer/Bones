using Bones.Database.DbSets.TaskManagement;

namespace Bones.Api.Models.Initiatives;

/// <summary>
///   Response for the GetTaskQueuesInInitiativeAsync endpoint
/// </summary>
[JsonSerializable(typeof(GetTaskQueuesInInitiativeResponse))]
public record GetTaskQueuesInInitiativeResponse
{
    /// <summary>
    ///   The ID of the queue
    /// </summary>
    public required Guid TaskQueueId { get; init; }

    /// <summary>
    ///   The name of the queue
    /// </summary>
    public required string TaskQueueName { get; init; }

    internal static List<GetTaskQueuesInInitiativeResponse> FromInternalList(List<TaskQueue> queues)
    {
        return [.. queues.Select(FromInternal)];
    }

    internal static GetTaskQueuesInInitiativeResponse FromInternal(TaskQueue queue)
    {
        return new()
        {
            TaskQueueId = queue.Id,
            TaskQueueName = queue.Name
        };
    }
}
