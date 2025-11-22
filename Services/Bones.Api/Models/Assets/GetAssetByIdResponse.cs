using Bones.Api.Models.Item;
using Bones.Database.DbSets.AssetManagement;

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
    public required int CurrentVersion { get; init; }

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

    /// <summary>
    ///   The values for this asset
    /// </summary>
    public required IEnumerable<ItemValueDisplayModel> ItemValues { get; init; }

    internal static GetAssetByIdResponse FromInternal(Asset asset)
    {
        return new()
        {
            AssetId = asset.Id,
            ProjectId = asset.Project.Id,
            Title = asset.Item.Versions.First(v => v.Version == asset.Item.CurrentVersion).Title,
            LayoutId = asset.Item.ItemLayout.Id,
            ItemId = asset.Item.Id,
            LatestItemVersionId = asset.Item.Current?.Id ?? throw new(),
            CurrentVersion = asset.Item.CurrentVersion,
            FriendlyId = asset.Item.FriendlyId,
            CreateDateTime = asset.Item.CreateDateTime,
            LatestVersionCreateDateTime = asset.Item.Current?.CreateDateTime ?? throw new(),
            ItemValues = asset.Item.Current.ItemLayoutVersion.FieldLinks.Select(fl => new ItemValueDisplayModel
            {
                OrderNumber = fl.OrderNumber,
                FieldVersionId = fl.FieldVersion.Id,
                Name = fl.FieldVersion.Name,
                ValueType = fl.FieldVersion.Type,
                IsRequired = fl.FieldVersion.IsRequired,
                CanBeNegative = fl.FieldVersion.CanBeNegative,
                PossibleValues = fl.FieldVersion.PossibleValues?.Select(v => v.Value),
                Value = asset.Item.Current.Values.FirstOrDefault(v => v.Field.Id == fl.FieldVersion.Id)?.Value
            })
        };
    }
}
