using System.ComponentModel.DataAnnotations;
using Bones.Database.DbSets.GenericItems;
using Bones.Shared.Backend.Enums;

namespace Bones.Api.Models.GenericItem;

/// <summary>
///   API response for the GetLatestItemLayoutVersionAsync endpoint
/// </summary>
[JsonSerializable(typeof(GetLatestItemLayoutVersionResponse))]
public sealed record GetLatestItemLayoutVersionResponse
{
    /// <summary>
    ///   Name of the item layout
    /// </summary>
    [JsonRequired]
    public required string Name { get; init; }

    /// <summary>
    ///   The uses for which this layout is enabled
    /// </summary>
    [JsonRequired]
    public required ItemLayoutUses EnabledFor { get; init; }

    /// <summary>
    ///   The prefix at the start of a Friendly ID for items using this layout, up to 6 letters.
    /// </summary>
    [JsonRequired]
    [MaxLength(6)]
    public required string FriendlyIdPrefix { get; init; }

    /// <summary>
    ///   The field versions
    /// </summary>
    [JsonRequired]
    public required Dictionary<uint, Guid> FieldVersions { get; init; }

    internal static GetLatestItemLayoutVersionResponse FromInternal(GenericItemLayout layout)
    {
        return new()
        {
            Name = layout.CurrentVersion?.Name ?? string.Empty,
            EnabledFor = layout.CurrentVersion?.EnabledFor ?? ItemLayoutUses.None,
            FriendlyIdPrefix = layout.FriendlyIdPrefix,
            FieldVersions = layout.CurrentVersion?.FieldLinks.ToDictionary(fl => fl.OrderNumber, fl => fl.FieldVersion.Id) ?? []
        };
    }
}
