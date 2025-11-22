using System.ComponentModel.DataAnnotations;
using Bones.Api.Models.Item;
using Bones.Database.DbSets.AccountManagement;
using Bones.Logic.Features.Item;
using Bones.Shared.Backend.Enums;

namespace Bones.Api.Models.Project;

/// <summary>
///   API request to create a new item layout
/// </summary>
public class CreateItemLayoutRequest
{
    /// <summary>
    ///   The ID of the project to create this layout in
    /// </summary>
    [JsonRequired]
    public required Guid ProjectId { get; init; }

    /// <summary>
    ///   Name of the item layout to create
    /// </summary>
    [JsonRequired]
    public required string Name { get; init; }

    /// <summary>
    ///   The use for which this layout is intended
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
    ///   The field versions to use for the initial layout version
    /// </summary>
    [JsonRequired]
    public required List<KeyValuePair<int, Guid>> FieldVersions { get; init; }

    /// <summary>
    ///   The assignee slots to use for the initial layout version
    /// </summary>
    [JsonRequired]
    public required List<ItemAssigneeSlotModel> AssigneeSlots { get; init; }

    internal CreateItemLayout.Command ToInternal(BonesUser user)
    {
        return new(ProjectId, Name, LayoutUse, FriendlyIdPrefix, FieldVersions.ToDictionary(), AssigneeSlots.ToDictionary(ad => ad.OrderNumber, ad => (ad.Name, ad.AssignmentType, ad.SelectionType, ad.States)), user);
    }
}
