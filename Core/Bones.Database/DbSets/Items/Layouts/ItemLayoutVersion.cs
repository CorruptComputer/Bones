using Bones.Database.DbConsts;
using Bones.Database.DbSets.Items.Assignments;
using Bones.Shared.Backend.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bones.Database.DbSets.Items.Layouts;

/// <summary>
///   Model for the Items.Layouts.ItemLayoutVersions table
/// </summary>
[Table(TableNames.Item.Layouts.ItemLayoutVersions, Schema = SchemaNames.Items_Layouts)]
[PrimaryKey(nameof(Id))]
public class ItemLayoutVersion
{
    /// <summary>
    ///   Internal ID for the ItemLayoutVersion
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; init; }

    /// <summary>
    ///   The name for this ItemLayoutVersion
    /// </summary>
    [MaxLength(512)]
    public required string Name { get; set; }

    /// <summary>
    ///   The usage this ItemLayoutVersion is applicable for
    /// </summary>
    public required ItemLayoutUse LayoutUse { get; set; }

    /// <summary>
    ///   The date and time this ItemLayoutVersion was created
    /// </summary>
    public DateTimeOffset CreateDateTime { get; init; } = DateTimeOffset.Now;

    /// <summary>
    ///   The ID of the layout this ItemLayoutVersion belongs to
    /// </summary>
    public required Guid ItemLayoutId { get; init; }

    /// <summary>
    ///   The version number for this
    /// </summary>
    public required long Version { get; init; }

    /// <summary>
    ///   Disables creating of new items using this layout version,
    ///   and when all items using it are deleted it will be removed.
    /// </summary>
    public bool DeleteFlag { get; set; } = false;

    #region Navigational Properties
    /// <summary>
    ///   The ID of the layout this version belongs to
    /// </summary>
    public ItemLayout? ItemLayout { get; init; }

    /// <summary>
    ///   The field links associated with this layout version
    /// </summary>
    public List<ItemLayoutFieldVersionLink> ItemLayoutFieldVersionLinks { get; init; } = [];

    /// <summary>
    ///   The assignee slots associated with this layout version
    /// </summary>
    public List<ItemAssignmentSlot> ItemAssignmentSlots { get; init; } = [];
    #endregion

    internal static void BuildTable(EntityTypeBuilder<ItemLayoutVersion> builder)
    {
        // Remove deleted items from being included in default queries
        builder.HasQueryFilter(x => !x.DeleteFlag);

        builder.HasOne(ilv => ilv.ItemLayout)
               .WithMany(il => il.Versions)
               .HasForeignKey(ilv => ilv.ItemLayoutId);

        builder.HasMany(ilv => ilv.ItemLayoutFieldVersionLinks)
               .WithOne(ilfvl => ilfvl.ItemLayoutVersion)
               .HasForeignKey(ilfvl => ilfvl.ItemLayoutVersionId);

        builder.HasMany(ilv => ilv.ItemAssignmentSlots)
               .WithOne(ias => ias.ItemLayoutVersion)
               .HasForeignKey(ias => ias.ItemLayoutVersionId);
    }
}