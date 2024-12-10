using System.Text.Json.Serialization;
using Bones.Shared.Backend.Enums;

namespace Bones.Api.Controllers;

public sealed partial class ProjectController
{
    /// <summary>
    ///   Response for the GetProjectQuickSelectAsync endpoint
    /// </summary>
    [JsonSerializable(typeof(GetProjectQuickSelectResponse))]
    public record GetProjectQuickSelectResponse
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
    }

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
        ///   The type of owner
        /// </summary>
        [JsonRequired]
        public required OwnershipType OwnerType { get; init; }

        /// <summary>
        ///   The ID of the owner
        /// </summary>
        [JsonRequired]
        public required Guid OwnerId { get; init; }

        /// <summary>
        ///   The number of initiatives in the project
        /// </summary>
        [JsonRequired]
        public required int InitiativeCount { get; init; }

        /// <summary>
        ///   A list of the initiatives in the project
        /// </summary>
        [JsonRequired]
        public required List<InitiativeListModel> Initiatives { get; init; }
    }

    /// <summary>
    ///   Model for the initiatives to be listed in a project
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