using Bones.Database.DbSets.GenericItems;
using Bones.Shared.Backend.Enums;

namespace Bones.Api.Models.GenericItem;

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
    public required Guid CurrentLayoutVersionId { get; init; }

    /// <summary>
    ///   The current version number of the layout
    /// </summary>
    public required long CurrentVersionNumber { get; init; }

    /// <summary>
    ///   Name of the layout
    /// </summary>
    public required string LayoutName { get; init; }

    /// <summary>
    ///   The friendly ID prefix for this layout
    /// </summary>
    public required string FriendlyIdPrefix { get; init; }

    internal static List<GetProjectLayoutsResponse> FromInternalList(List<GenericItemLayout> layouts, ItemLayoutUses? enabledFor)
    {
        if (enabledFor == null)
        {
            return [.. layouts.Select(FromInternal)];
        }

        return [.. layouts.Where(l => ((l.CurrentVersion?.EnabledFor ?? ItemLayoutUses.None) & enabledFor) != ItemLayoutUses.None).Select(FromInternal)];
    }

    internal static GetProjectLayoutsResponse FromInternal(GenericItemLayout layout)
    {
        return new()
        {
            LayoutId = layout.Id,
            CurrentLayoutVersionId = layout.CurrentVersion?.Id ?? Guid.Empty,
            CurrentVersionNumber = layout.CurrentVersion?.Version ?? 0,
            LayoutName = layout.CurrentVersion?.Name ?? "Broken",
            FriendlyIdPrefix = layout.FriendlyIdPrefix
        };
    }
}
