using Bones.Api.Models.Item;
using Bones.Database.DbSets.Items;
using Bones.Database.DbSets.Items.Layouts;

namespace Bones.Api.Models.ItemVersions;

/// <summary>
///   API response for the GetItemVersionByIdAsync endpoint
/// </summary>
public class GetItemVersionByIdResponse
{
    /// <summary>
    ///   The values for this item version
    /// </summary>
    public required IEnumerable<ItemValueDisplayModel> ItemValues { get; init; }

    internal static GetItemVersionByIdResponse FromInternal(ItemVersion itemVersion)
    {
        List<ItemValue> itemValues = itemVersion.ItemValues;
        List<ItemLayoutFieldVersionLink> itemLayoutFieldVersionLinks = itemVersion.ItemLayoutVersion!.ItemLayoutFieldVersionLinks;

        return new()
        {
            ItemValues = itemLayoutFieldVersionLinks.Select(
                fl =>
                {
                    ItemValue value = itemValues.First(v => v.ItemFieldVersionId == fl.ItemFieldVersionId);
                    return ItemValueDisplayModel.FromInternal(fl.OrderNumber, value.ItemFieldVersion!, value);
                }
            )
        };
    }
}
