using Bones.Database.DbConsts;
using Bones.Database.DbSets.Items.Layouts;
using Bones.Database.DbSets.Projects;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bones.Database.DbSets.Items;

/// <summary>
///   Model for the Item.Items table
/// </summary>
[Table(TableNames.Item.Items, Schema = SchemaNames.Items)]
[PrimaryKey(nameof(Id))]
[Index(nameof(FriendlyId))]
public class Item
{
    /// <summary>
    ///   Internal ID for the Item
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; init; }

    /// <summary>
    ///   The friendly ID for this item
    /// </summary>
    public required string FriendlyId { get; init; }

    /// <summary>
    ///   The date and time this item was created
    /// </summary>
    public DateTimeOffset CreateDateTime { get; init; } = DateTimeOffset.Now;

    /// <summary>
    ///   The ID of the project this item belongs to
    /// </summary>
    public required Guid ProjectId { get; init; }

    /// <summary>
    ///   The ID of the Item Layout this item is made from
    /// </summary>
    public required Guid ItemLayoutId { get; init; }

    /// <summary>
    ///   The version this item is currently using
    /// </summary>
    public long CurrentVersion { get; set; } = 1;

    /// <summary>
    ///   Disables access to this item and schedules deletion.
    /// </summary>
    public bool DeleteFlag { get; set; } = false;

    #region Navigational Properties
    /// <summary>
    ///   Navigational property for the project this item belongs to, null if not .Include()'d in the query
    /// </summary>
    public Project? Project { get; set; }

    /// <summary>
    ///   Navigational property for the item layout this item is made from, null if not .Include()'d in the query
    /// </summary>
    public ItemLayout? ItemLayout { get; set; }

    /// <summary>
    ///   Navigational property for the versions of this item, emtpy if not .Includ()'d in the query
    /// </summary>
    public List<ItemVersion> Versions { get; set; } = [];
    #endregion

    #region Non-Mapped Properties
    /// <summary>
    ///   The current version of this item, null if there are no versions
    /// </summary>
    [NotMapped]
    public ItemVersion? Current => Versions.OrderByDescending(v => v.Version).FirstOrDefault();
    #endregion

    internal static void BuildTable(EntityTypeBuilder<Item> builder)
    {
        // Remove deleted items from being included in default queries
        builder.HasQueryFilter(i => !i.DeleteFlag);

        builder.HasOne(i => i.Project)
               .WithMany()
               .HasForeignKey(i => i.ProjectId);

        builder.HasOne(i => i.ItemLayout)
               .WithMany()
               .HasForeignKey(i => i.ItemLayoutId);

        builder.HasMany(i => i.Versions)
               .WithOne(iv => iv.Item)
               .HasForeignKey(iv => iv.ItemId);
    }
}