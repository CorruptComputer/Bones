namespace Bones.Api.Models.Project;

/// <summary>
///   Response for the GetInitiativeDashboardAsync endpoint
/// </summary>
[JsonSerializable(typeof(GetInitiativeDashboardResponse))]
public record GetInitiativeDashboardResponse
{
    /// <summary>
    ///   The initiative ID
    /// </summary>
    [JsonRequired]
    public required Guid InitiativeId { get; init; }

    /// <summary>
    ///   The name of the Initiative
    /// </summary>
    [JsonRequired]
    public required string InitiativeName { get; init; }

    /// <summary>
    ///   The ID of the project the initiative is in
    /// </summary>
    [JsonRequired]
    public required Guid ProjectId { get; init; }

    /// <summary>
    ///   The number of initiatives in the project
    /// </summary>
    [JsonRequired]
    public required int TaskQueueCount { get; init; }

    /// <summary>
    ///   A list of the initiatives in the project
    /// </summary>
    [JsonRequired]
    public required IEnumerable<TaskQueueListModel> TaskQueues { get; init; }

    /// <summary>
    ///   Model for the  queues to be listed in an initiative
    /// </summary>
    [JsonSerializable(typeof(TaskQueueListModel))]
    public sealed record TaskQueueListModel
    {
        /// <summary>
        ///   The  queue ID
        /// </summary>
        [JsonRequired]
        public required Guid TaskQueueId { get; init; }

        /// <summary>
        ///   The name of the  queue
        /// </summary>
        [JsonRequired]
        public required string TaskQueueName { get; init; }

        /// <summary>
        ///   The number of s in the  queue
        /// </summary>
        [JsonRequired]
        public required int TaskCount { get; init; }
    }
}
