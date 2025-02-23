using Bones.Database.DbSets.WorkItemManagement;

namespace Bones.Api.Models.WorkItems;

/// <summary>
///   Response for the GetWorkItemQueueByIdAsync endpoint
/// </summary>
[JsonSerializable(typeof(GetWorkItemQueueByIdResponse))]
public record GetWorkItemQueueByIdResponse
{
    /// <summary>
    ///   The name of the queue
    /// </summary>
    public required string QueueName { get; init; }

    internal static GetWorkItemQueueByIdResponse FromInternal(WorkItemQueue queue)
    {
        return new()
        {
            QueueName = queue.Name
        };
    }
}
