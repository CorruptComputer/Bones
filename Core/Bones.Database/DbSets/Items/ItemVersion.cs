using Bones.Database.DbConsts;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bones.Database.DbSets.Items;

/// <summary>
///   Model for the Item.ItemVersions table
/// </summary>
[Table(TableNames.Item.ItemVersions, Schema = SchemaNames.Item)]
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
    ///   The Item this version belongs to
    /// </summary>
    public required Item Item { get; init; }

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
    ///   The layout version this item version uses
    /// </summary>
    public required ItemLayoutVersion ItemLayoutVersion { get; set; }

    /// <summary>
    ///   The values for the fields defined by this items layout version
    /// </summary>
    public required List<ItemValue> Values { get; set; }

    /// <summary>
    ///   The assignees for this item version
    /// </summary>
    public required List<ItemAssignee> Assignees { get; set; }

    /// <summary>
    ///   Disables viewing this item version, and when safe to do so it will be deleted.
    /// </summary>
    public bool DeleteFlag { get; set; } = false;

    internal static void BuildTable(EntityTypeBuilder<ItemVersion> builder)
    {
        // Remove deleted items from being included in default queries
        builder.HasQueryFilter(x => !x.DeleteFlag);
    }
}