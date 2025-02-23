using Bones.Database.DbSets.WorkItemManagement;

namespace Bones.Api.Models.Initiatives;

/// <summary>
///   Response for the GetWorkItemQueuesInInitiativeAsync endpoint
/// </summary>
[JsonSerializable(typeof(GetWorkItemQueuesInInitiativeResponse))]
public record GetWorkItemQueuesInInitiativeResponse
{
    /// <summary>
    ///   The ID of the queue
    /// </summary>
    public required Guid QueueId { get; init; }

    /// <summary>
    ///   The name of the queue
    /// </summary>
    public required string QueueName { get; init; }

    internal static List<GetWorkItemQueuesInInitiativeResponse> FromInternalList(List<WorkItemQueue> queues)
    {
        return [.. queues.Select(FromInternal)];
    }

    internal static GetWorkItemQueuesInInitiativeResponse FromInternal(WorkItemQueue queue)
    {
        return new()
        {
            QueueId = queue.Id,
            QueueName = queue.Name
        };
    }
}
