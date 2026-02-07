using Bones.Database.DbSets.Projects;

namespace Bones.Api.Models.TaskQueues;

/// <summary>
///   Response for the GetTaskQueueByIdAsync endpoint
/// </summary>
[JsonSerializable(typeof(GetTaskQueueByIdResponse))]
public sealed record GetTaskQueueByIdResponse
{
    /// <summary>
    ///   The name of the queue
    /// </summary>
    public required string QueueName { get; init; }

    /// <summary>
    ///   The ID of the project this queue belongs to
    /// </summary>
    public required Guid ProjectId { get; init; }

    /// <summary>
    ///   The ID of the initiative this queue belongs to
    /// </summary>
    public required Guid InitiativeId { get; init; }

    internal static GetTaskQueueByIdResponse FromInternal(TaskQueue queue)
    {
        return new()
        {
            QueueName = queue.Name,
            ProjectId = queue.Initiative!.ProjectId,
            InitiativeId = queue.InitiativeId,
        };
    }
}
