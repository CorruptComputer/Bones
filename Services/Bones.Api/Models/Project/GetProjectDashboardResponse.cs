using Bones.Shared.Backend.Enums;

namespace Bones.Api.Models.Project;

/// <summary>
///   Response for the GetProjectDashboardAsync endpoint
/// </summary>
[JsonSerializable(typeof(GetProjectDashboardResponse))]
public record GetProjectDashboardResponse
{
    /// <summary>
    ///   The projects ID
    /// </summary>
    [JsonRequired]
    public required Guid ProjectId { get; init; }

    /// <summary>
    ///   The name of the project
    /// </summary>
    [JsonRequired]
    public required string ProjectName { get; init; }

    /// <summary>
    ///   The number of initiatives in the project
    /// </summary>
    [JsonRequired]
    public required int InitiativeCount { get; init; }

    /// <summary>
    ///   A list of the initiatives in the project
    /// </summary>
    [JsonRequired]
    public required IEnumerable<InitiativeListModel> Initiatives { get; init; }

    /// <summary>
    ///   Model for the initiatives to be listed in a project's dashboard
    /// </summary>
    [JsonSerializable(typeof(InitiativeListModel))]
    public sealed record InitiativeListModel
    {
        /// <summary>
        ///   The initiatives ID
        /// </summary>
        [JsonRequired]
        public required Guid InitiativeId { get; init; }

        /// <summary>
        ///   The name of the initiative
        /// </summary>
        [JsonRequired]
        public required string InitiativeName { get; init; }

        /// <summary>
        ///   The number of queues in the initiative
        /// </summary>
        [JsonRequired]
        public required int QueueCount { get; init; }
    }
}

