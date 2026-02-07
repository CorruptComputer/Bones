using Bones.Database.DbConsts;
using Bones.Database.DbSets.Items.Assignments;
using Bones.Database.DbSets.Items.Layouts;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bones.Database.DbSets.Items;

/// <summary>
///   Model for the Item.ItemVersions table
/// </summary>
[Table(TableNames.Item.ItemVersions, Schema = SchemaNames.Items)]
[PrimaryKey(nameof(Id))]
[Index(nameof(Version))]
public class ItemVersion
{
    /// <summary>
    ///   Internal ID for the ItemVersion
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    ///   The ID of the Item this version belongs to
    /// </summary>
    public required Guid ItemId { get; init; }

    /// <summary>
    ///   The title of this version of the item
    /// </summary>
    [MaxLength(256)]
    public required string Title { get; init; }

    /// <summary>
    ///   The version number for this
    /// </summary>
    public required long Version { get; init; }

    /// <summary>
    ///   The time which this was created
    /// </summary>
    public required DateTimeOffset CreateDateTime { get; init; }

    /// <summary>
    ///   The ID of the ItemLayoutVersion this version uses
    /// </summary>
    public required Guid ItemLayoutVersionId { get; init; }

    /// <summary>
    ///   Disables viewing this item version, and when safe to do so it will be deleted.
    /// </summary>
    public bool DeleteFlag { get; set; } = false;

    #region Navigational Properties
    /// <summary>
    ///   Navigational property for the Item, null if not .Include()'d in the query
    /// </summary>
    public Item? Item { get; init; }

    /// <summary>
    ///   Navigational property for the ItemLayoutVersion, null if not .Include()'d in the query
    /// </summary>
    public ItemLayoutVersion? ItemLayoutVersion { get; set; }

    /// <summary>
    ///   Navigational property for the Values, empty if not .Include()'d in the query
    /// </summary>
    public List<ItemValue> ItemValues { get; set; } = [];

    /// <summary>
    ///   Navigational property for the Assignees, empty if not .Include()'d in the query
    /// </summary>
    public List<ItemAssignee> ItemAssignees { get; set; } = [];
    #endregion

    internal static void BuildTable(EntityTypeBuilder<ItemVersion> builder)
    {
        // Remove deleted items from being included in default queries
        builder.HasQueryFilter(iver => !iver.DeleteFlag);

        builder.HasOne(iver => iver.Item)
            .WithMany(i => i.Versions)
            .HasForeignKey(iver => iver.ItemId);

        builder.HasOne(iver => iver.ItemLayoutVersion)
            .WithMany()
            .HasForeignKey(iver => iver.ItemLayoutVersionId);

        builder.HasMany(iver => iver.ItemValues)
               .WithOne(ival => ival.ItemVersion)
               .HasForeignKey(ival => ival.ItemVersionId);

        builder.HasMany(iver => iver.ItemAssignees)
               .WithOne(ia => ia.ItemVersion)
               .HasForeignKey(ia => ia.ItemVersionId);
    }
}