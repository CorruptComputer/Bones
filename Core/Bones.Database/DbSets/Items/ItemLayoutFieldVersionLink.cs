using Bones.Database.DbConsts;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bones.Database.DbSets.Items;

/// <summary>
///   Model for the Item.ItemLayoutFieldVersionLinks table
/// </summary>
[Table(TableNames.Item.ItemLayoutFieldVersionLinks, Schema = SchemaNames.Item)]
[PrimaryKey(nameof(Id))]
public class ItemLayoutFieldVersionLink
{
    /// <summary>
    ///   Internal ID for the GenericItemLayoutFieldVersionLink
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; init; }

    /// <summary>
    ///   The order number for which this field should be displayed
    /// </summary>
    public required int OrderNumber { get; set; }

    /// <summary>
    ///   The layout version for this link
    /// </summary>
    public required ItemLayoutVersion LayoutVersion { get; set; }

    /// <summary>
    ///   The field version for this link
    /// </summary>
    public required ItemFieldVersion FieldVersion { get; set; }

    /// <summary>
    ///   Disables usage of this, and when all items using it are deleted it will be removed.
    /// </summary>
    public bool DeleteFlag { get; set; } = false;

    internal static void BuildTable(EntityTypeBuilder<ItemLayoutFieldVersionLink> builder)
    {
        // Remove deleted items from being included in default queries
        builder.HasQueryFilter(x => !x.DeleteFlag);
    }
}
