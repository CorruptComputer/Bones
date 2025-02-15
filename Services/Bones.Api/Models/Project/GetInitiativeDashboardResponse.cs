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
    public required int WorkItemQueueCount { get; init; }

    /// <summary>
    ///   A list of the initiatives in the project
    /// </summary>
    [JsonRequired]
    public required IEnumerable<WorkItemQueueListModel> WorkItemQueues { get; init; }

    /// <summary>
    ///   Model for the work item queues to be listed in an initiative
    /// </summary>
    [JsonSerializable(typeof(WorkItemQueueListModel))]
    public sealed record WorkItemQueueListModel
    {
        /// <summary>
        ///   The work item queue ID
        /// </summary>
        [JsonRequired]
        public required Guid WorkItemQueueId { get; init; }

        /// <summary>
        ///   The name of the work item queue
        /// </summary>
        [JsonRequired]
        public required string WorkItemQueueName { get; init; }

        /// <summary>
        ///   The number of work items in the work item queue
        /// </summary>
        [JsonRequired]
        public required int WorkItemCount { get; init; }
    }
}
