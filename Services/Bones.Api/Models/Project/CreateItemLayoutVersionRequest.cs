using Bones.Database.DbSets.AccountManagement;
using Bones.Logic.Features.Item;
using Bones.Shared.Backend.Enums;

namespace Bones.Api.Models.Project;

/// <summary>
///   API request to create a new item layout version
/// </summary>
public class CreateItemLayoutVersionRequest
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
    public required ItemLayoutUse LayoutUse { get; init; }

    /// <summary>
    ///   The field versions to use for this layout version
    /// </summary>
    [JsonRequired]
    public required List<KeyValuePair<int, Guid>> FieldVersions { get; init; }

    internal CreateItemLayoutVersion.Command ToInternal(Guid projectId, BonesUser user)
    {
        return new(projectId, Name, LayoutUse, FieldVersions.ToDictionary(), user);
    }
}
