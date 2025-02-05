namespace Bones.Api.Models.Project;

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
