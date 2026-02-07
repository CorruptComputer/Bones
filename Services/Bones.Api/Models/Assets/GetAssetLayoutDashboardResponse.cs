using Bones.Database.DbSets.Items.Layouts;
using Bones.Database.DbSets.Items.Types;

namespace Bones.Api.Models.Assets;

/// <summary>
///   Response for the GetAssetLayoutDashboardAsync endpoint
/// </summary>
[JsonSerializable(typeof(GetAssetLayoutDashboardResponse))]
public sealed record GetAssetLayoutDashboardResponse
{
    /// <summary>
    ///   The name of the layout
    /// </summary>
    public required string LayoutName { get; init; }

    /// <summary>
    ///   The assets in this layout, ordered by date added from oldest to newest
    /// </summary>
    public required IEnumerable<DashboardAssetModel> Assets { get; init; }


    internal static GetAssetLayoutDashboardResponse FromInternal(ItemLayout layout, IEnumerable<Asset> assets)
    {
        return new()
        {
            LayoutName = layout.Current?.Name ?? string.Empty,
            Assets = assets.Select(DashboardAssetModel.FromAsset)
        };
    }

    /// <summary>
    ///   Model for a  in the dashboard
    /// </summary>
    public record DashboardAssetModel
    {
        /// <summary>
        ///   The ID of the
        /// </summary>
        public required Guid Id { get; init; }

        /// <summary>
        ///   The friendly ID of the
        /// </summary>
        public required string FriendlyId { get; init; }

        /// <summary>
        ///   The title of the
        /// </summary>
        public required string Title { get; init; }

        /// <summary>
        ///   The date and time the latest version of the asset was created
        /// </summary>
        public required DateTimeOffset LatestVersionCreateDateTime { get; init; }

        internal static DashboardAssetModel FromAsset(Asset asset)
        {
            return new()
            {
                Id = asset.Id,
                FriendlyId = asset.Item?.FriendlyId  ?? string.Empty,
                Title = asset.Item?.Current?.Title ?? string.Empty,
                LatestVersionCreateDateTime = asset.Item?.Current?.CreateDateTime ?? DateTimeOffset.MinValue
            };
        }
    }
}
