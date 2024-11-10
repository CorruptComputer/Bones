using System.Text.Json.Serialization;
using Bones.Shared.Backend.Enums;

namespace Bones.Api.Controllers;

public sealed partial class ProjectController
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="ProjectId"></param>
    /// <param name="ProjectName"></param>
    [Serializable]
    [JsonSerializable(typeof(GetProjectQuickSelectResponse))]
    public record GetProjectQuickSelectResponse(
        Guid ProjectId,
        string ProjectName
    );

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ProjectId"></param>
    /// <param name="ProjectName"></param>
    /// <param name="OwnerType"></param>
    /// <param name="OwnerId"></param>
    /// <param name="InitiativeCount"></param>
    /// <param name="Initiatives"></param>
    [Serializable]
    [JsonSerializable(typeof(GetProjectDashboardResponse))]
    public record GetProjectDashboardResponse(
        Guid ProjectId,
        string ProjectName,
        OwnershipType OwnerType,
        Guid OwnerId,
        int InitiativeCount,
        List<InitiativeListModel> Initiatives
    );

    /// <summary>
    /// 
    /// </summary>
    /// <param name="InitiativeId"></param>
    /// <param name="InitiativeName"></param>
    /// <param name="QueueCount"></param>
    [Serializable]
    [JsonSerializable(typeof(InitiativeListModel))]
    public sealed record InitiativeListModel(
        Guid InitiativeId,
        string InitiativeName,
        int QueueCount
    );
}