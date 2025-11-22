using System.Runtime.Serialization;
using Bones.Database.DbConsts;
using Bones.Database.DbSets.ProjectManagement;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bones.Database.DbSets.Items;

/// <summary>
///   Model for the Item.Items table
/// </summary>
[Table(TableNames.Item.Items, Schema = SchemaNames.Item)]
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
    ///   The project this item belongs to
    /// </summary>
    public required Project Project { get; set; }

    /// <summary>
    ///   The layout this item will use
    /// </summary>
    public required ItemLayout ItemLayout { get; set; }

    /// <summary>
    ///   The versions for this item
    /// </summary>
    public List<ItemVersion> Versions { get; set; } = [];

    /// <summary>
    ///   The current version of the this
    /// </summary>
    [NotMapped]
    public ItemVersion? Current => Versions.OrderByDescending(v => v.Version).FirstOrDefault();

    /// <summary>
    ///   The version this item is currently using
    /// </summary>
    public int CurrentVersion { get; set; } = 1;

    /// <summary>
    ///   Disables access to this item and schedules deletion.
    /// </summary>
    public bool DeleteFlag { get; set; } = false;

    internal static void BuildTable(EntityTypeBuilder<Item> builder)
    {
        // Remove deleted items from being included in default queries
        builder.HasQueryFilter(x => !x.DeleteFlag);
    }
}