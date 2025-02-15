using System.ComponentModel.DataAnnotations;
using Bones.Database.DbSets.AccountManagement;
using Bones.Logic.Features.GenericItem;
using Bones.Shared.Backend.Enums;

namespace Bones.Api.Models.Project;

/// <summary>
///   API request to create a new item layout
/// </summary>
public class CreateItemLayoutRequest
{
    /// <summary>
    ///   Name of the item layout to create
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
    ///   The field versions to use for the initial layout version
    /// </summary>
    [JsonRequired]
    public required List<Guid> FieldVersions { get; init; }

    internal CreateItemLayout.Command ToInternal(Guid projectId, BonesUser user)
    {
        return new(projectId, Name, EnabledFor, FriendlyIdPrefix, FieldVersions, user);
    }
}
