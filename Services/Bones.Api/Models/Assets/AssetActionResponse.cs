namespace Bones.Api.Models.Assets;

/// <summary>
///   Response model for asset actions.
/// </summary>
[JsonSerializable(typeof(AssetActionResponse))]
public sealed record AssetActionResponse
{
    /// <summary>
    ///   The unique identifier of the asset action.
    /// </summary>
    public required Guid AssetId { get; init; }

    /// <summary>
    ///   The unique identifier of the current version of the asset.
    /// </summary>
    public required Guid AssetCurrentVersionId { get; init; }
}
