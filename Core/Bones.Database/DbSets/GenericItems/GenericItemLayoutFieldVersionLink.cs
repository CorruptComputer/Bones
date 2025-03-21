using Bones.Database.DbConsts;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bones.Database.DbSets.GenericItems;

/// <summary>
///     Model for the GenericItems.GenericItemLayoutFieldVersionLinks table
/// </summary>
[Table(TableNames.GenericItem.GenericItemLayoutFieldVersionLinks, Schema = SchemaNames.GenericItem)]
[PrimaryKey(nameof(Id))]
public class GenericItemLayoutFieldVersionLink
{
    /// <summary>
    ///     Internal ID for the GenericItemLayoutFieldVersionLink
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; init; }

    /// <summary>
    ///   The order number for which this field should be displayed
    /// </summary>
    public required uint OrderNumber { get; set; }

    /// <summary>
    ///   The layout version for this link
    /// </summary>
    public required GenericItemLayoutVersion LayoutVersion { get; set; }

    /// <summary>
    ///   The field version for this link
    /// </summary>
    public required GenericItemFieldVersion FieldVersion { get; set; }

    /// <summary>
    ///   Disables usage of this, and when all items using it are deleted it will be removed.
    /// </summary>
    public bool DeleteFlag { get; set; } = false;

    internal static void BuildTable(EntityTypeBuilder<GenericItemLayoutFieldVersionLink> builder)
    {
        // Remove deleted items from being included in default queries
        builder.HasQueryFilter(x => !x.DeleteFlag);
    }
}
