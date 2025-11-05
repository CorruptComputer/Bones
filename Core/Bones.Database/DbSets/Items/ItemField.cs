using Bones.Database.DbConsts;
using Bones.Database.DbSets.ProjectManagement;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bones.Database.DbSets.Items;

/// <summary>
///   Model for the Item.ItemFields table
/// </summary>
[Table(TableNames.Item.ItemFields, Schema = SchemaNames.Item)]
[PrimaryKey(nameof(Id))]
public class ItemField
{
    /// <summary>
    ///   Internal ID for the ItemField
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    ///   The date and time this field was created
    /// </summary>
    public DateTimeOffset CreateDateTime { get; init; } = DateTimeOffset.Now;

    /// <summary>
    ///   The project this ItemField belongs to
    /// </summary>
    public required Project Project { get; set; }

    /// <summary>
    ///   The latest version of this field
    /// </summary>
    [NotMapped]
    public ItemFieldVersion? LatestVersion => Versions.OrderByDescending(v => v.Version).FirstOrDefault();

    /// <summary>
    ///   The versions for this Item field
    /// </summary>
    public List<ItemFieldVersion> Versions { get; set; } = [];

    /// <summary>
    ///   Disables creating of new layouts with this field,
    ///   and when all items using it are deleted it will be removed.
    /// </summary>
    public bool DeleteFlag { get; set; } = false;

    internal static void BuildTable(EntityTypeBuilder<ItemField> builder)
    {
        // Remove deleted items from being included in default queries
        builder.HasQueryFilter(x => !x.DeleteFlag);
    }
}