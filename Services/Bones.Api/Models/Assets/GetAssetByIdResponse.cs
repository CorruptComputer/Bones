using Bones.Api.Models.Item;
using Bones.Database.DbSets.AssetManagement;
using Bones.Database.DbSets.Items;

namespace Bones.Api.Models.Assets;

/// <summary>
///   Response for the GetAssetById endpoint
/// </summary>
[JsonSerializable(typeof(GetAssetByIdResponse))]
public sealed record GetAssetByIdResponse
{
    /// <summary>
    ///   ID of the asset
    /// </summary>
    public required Guid AssetId { get; init; }

    /// <summary>
    ///   The title of the asset
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    ///   The ID of the layout this asset uses
    /// </summary>
    public required Guid LayoutId { get; init; }

    /// <summary>
    ///   The ID of the layout version the latest version of this asset uses
    /// </summary>
    public required Guid LayoutVersionId { get; init; }

    /// <summary>
    ///   The ID of the project this asset belongs to
    /// </summary>
    public required Guid ProjectId { get; init; }

    /// <summary>
    ///   The ID of the item this asset is
    /// </summary>
    public required Guid ItemId { get; init; }

    /// <summary>
    ///   The ID of the latest version of the item this asset is
    /// </summary>
    public required Guid LatestItemVersionId { get; init; }

    /// <summary>
    ///   The current version number for this asset
    /// </summary>
    public required long CurrentVersion { get; init; }

    /// <summary>
    ///   The friendly ID of the asset
    /// </summary>
    public required string FriendlyId { get; init; }

    /// <summary>
    ///   The time this asset was created
    /// </summary>
    public required DateTimeOffset CreateDateTime { get; init; }

    /// <summary>
    ///   The time the latest version of this asset was created
    /// </summary>
    public required DateTimeOffset LatestVersionCreateDateTime { get; init; }

    internal static GetAssetByIdResponse FromInternal(Asset asset)
    {
        ItemVersion? currentItem = asset.Item!.Current;
        if (currentItem is null)
        {
            throw new ArgumentNullException(nameof(currentItem), "Current item version is null");
        }

        return new()
        {
            AssetId = asset.Id,
            ProjectId = asset.ProjectId,
            Title = currentItem.Title,
            LayoutId = asset.Item.ItemLayoutId,
            LayoutVersionId = currentItem.ItemLayoutVersionId,
            ItemId = asset.ItemId,
            LatestItemVersionId = currentItem.Id,
            CurrentVersion = asset.Item.CurrentVersion,
            FriendlyId = asset.Item.FriendlyId,
            CreateDateTime = asset.Item.CreateDateTime,
            LatestVersionCreateDateTime = currentItem.CreateDateTime,
        };
    }
}
