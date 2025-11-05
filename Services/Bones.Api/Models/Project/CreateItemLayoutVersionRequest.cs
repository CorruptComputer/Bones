using Bones.Api.Models.Item;
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

    /// <summary>
    ///   The assignee definitions to use for the initial layout version
    /// </summary>
    [JsonRequired]
    public required List<ItemAssigneeDefinitionModel> AssigneeDefinitions { get; init; }

    internal CreateItemLayoutVersion.Command ToInternal(Guid projectId, BonesUser user)
    {
        return new(projectId, Name, LayoutUse, FieldVersions.ToDictionary(), AssigneeDefinitions.ToDictionary(ad => ad.OrderNumber, ad => (ad.Name, ad.AssignmentType, ad.SelectionType)), user);
    }
}
