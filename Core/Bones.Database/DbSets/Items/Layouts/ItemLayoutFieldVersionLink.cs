using Bones.Database.DbConsts;
using Bones.Database.DbSets.Items.Fields;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bones.Database.DbSets.Items.Layouts;

/// <summary>
///   Model for the Items.Layouts.ItemLayoutFieldVersionLinks table
/// </summary>
[Table(TableNames.Item.Layouts.ItemLayoutFieldVersionLinks, Schema = SchemaNames.Items_Layouts)]
[PrimaryKey(nameof(Id))]
public class ItemLayoutFieldVersionLink
{
    /// <summary>
    ///   Internal ID for the ItemLayoutFieldVersionLink
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; init; }

    /// <summary>
    ///   The order number for which this field should be displayed
    /// </summary>
    public required int OrderNumber { get; set; }

    /// <summary>
    ///   The ID of the layout version this link is pointing to
    /// </summary>
    public required Guid ItemLayoutVersionId { get; init; }

    /// <summary>
    ///   The ID of the field version this link is pointing to
    /// </summary>
    public required Guid ItemFieldVersionId { get; init; }

    /// <summary>
    ///   Disables usage of this, and when all items using it are deleted it will be removed.
    /// </summary>
    public bool DeleteFlag { get; set; } = false;

    #region Navigational Properties
    /// <summary>
    ///   The layout version for this link
    /// </summary>
    public ItemLayoutVersion? ItemLayoutVersion { get; set; }

    /// <summary>
    ///   The field version for this link
    /// </summary>
    public ItemFieldVersion? ItemFieldVersion { get; set; }
    #endregion

    internal static void BuildTable(EntityTypeBuilder<ItemLayoutFieldVersionLink> builder)
    {
        // Remove deleted items from being included in default queries
        builder.HasQueryFilter(x => !x.DeleteFlag);

        builder.HasOne(ilfvl => ilfvl.ItemLayoutVersion)
               .WithMany()
               .HasForeignKey(ilfvl => ilfvl.ItemLayoutVersionId);

        builder.HasOne(ilfvl => ilfvl.ItemFieldVersion)
               .WithMany()
               .HasForeignKey(ilfvl => ilfvl.ItemFieldVersionId);
    }
}
