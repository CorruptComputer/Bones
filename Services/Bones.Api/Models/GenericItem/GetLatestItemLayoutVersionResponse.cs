using System.ComponentModel.DataAnnotations;
using Bones.Database.DbSets.GenericItems;
using Bones.Shared.Enums;

namespace Bones.Api.Models.GenericItem;

/// <summary>
///   API response for the GetLatestItemLayoutVersionAsync endpoint
/// </summary>
[JsonSerializable(typeof(GetItemLayoutVersionResponse))]
public sealed record GetItemLayoutVersionResponse
{
    /// <summary>
    ///   Name of the item layout
    /// </summary>
    [JsonRequired]
    public required string Name { get; init; }

    /// <summary>
    ///   The ID of the project this layout belongs to
    /// </summary>
    [JsonRequired]
    public required Guid ProjectId { get; init; }

    /// <summary>
    ///   The uses for which this layout is enabled
    /// </summary>
    [JsonRequired]
    public required ItemLayoutUse LayoutUse { get; init; }

    /// <summary>
    ///   The prefix at the start of a Friendly ID for items using this layout, up to 6 letters.
    /// </summary>
    [JsonRequired]
    [MaxLength(6)]
    public required string FriendlyIdPrefix { get; init; }

    /// <summary>
    ///   The version number for this ItemLayoutVersion
    /// </summary>
    public required long Version { get; init; }

    /// <summary>
    ///   The current latest version of this layout
    /// </summary>
    public required long LatestVersion { get; init; }

    /// <summary>
    ///   Is there a version update available for this layout version?
    /// </summary>
    public bool VersionUpdateAvailable => LatestVersion > Version;

    /// <summary>
    ///   The field versions
    /// </summary>
    [JsonRequired]
    public required Dictionary<uint, Guid> FieldVersions { get; init; }

    internal static GetItemLayoutVersionResponse FromInternal(GenericItemLayoutVersion layoutVersion, Guid ProjectId, string friendlyIdPrefix, long latestVersion)
    {
        return new()
        {
            Name = layoutVersion.Name,
            ProjectId = ProjectId,
            LayoutUse = layoutVersion.LayoutUse,
            FriendlyIdPrefix = friendlyIdPrefix,
            Version = layoutVersion.Version,
            LatestVersion = latestVersion,
            FieldVersions = layoutVersion.FieldLinks.ToDictionary(fl => fl.OrderNumber, fl => fl.FieldVersion.Id)
        };
    }
}
