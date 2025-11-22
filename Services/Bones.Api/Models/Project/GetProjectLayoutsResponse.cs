using Bones.Database.DbSets.Items;
using Bones.Shared.Backend.Enums;

namespace Bones.Api.Models.Project;

/// <summary>
///   API response for the GetLatestItemLayoutVersionAsync endpoint
/// </summary>
[JsonSerializable(typeof(GetProjectLayoutsResponse))]
public sealed record GetProjectLayoutsResponse
{
    /// <summary>
    ///   ID of the layout
    /// </summary>
    public required Guid LayoutId { get; init; }

    /// <summary>
    ///   ID of the current layout version
    /// </summary>
    public required Guid LatestLayoutVersionId { get; init; }

    /// <summary>
    ///   The current version number of the layout
    /// </summary>
    public required long LatestVersionNumber { get; init; }

    /// <summary>
    ///   Name of the layout
    /// </summary>
    public required string LayoutName { get; init; }

    /// <summary>
    ///   The friendly ID prefix for this layout
    /// </summary>
    public required string FriendlyIdPrefix { get; init; }

    internal static List<GetProjectLayoutsResponse> FromInternalList(List<ItemLayout> layouts, ItemLayoutUse? layoutUse)
    {
        if (layoutUse is null)
        {
            return [.. layouts.Select(FromInternal)];
        }

        return [.. layouts.Where(l => (l.Current?.LayoutUse ?? ItemLayoutUse.None) == layoutUse).Select(FromInternal)];
    }

    internal static GetProjectLayoutsResponse FromInternal(ItemLayout layout)
    {
        return new()
        {
            LayoutId = layout.Id,
            LatestLayoutVersionId = layout.Current?.Id ?? Guid.Empty,
            LatestVersionNumber = layout.Current?.Version ?? 0,
            LayoutName = layout.Current?.Name ?? "Broken",
            FriendlyIdPrefix = layout.FriendlyIdPrefix
        };
    }
}
