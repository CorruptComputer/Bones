using Bones.Database.DbConsts;
using Bones.Database.DbSets.ProjectManagement;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bones.Database.DbSets.GenericItems;

/// <summary>
///     Model for the GenericItems.GenericItemFields table
/// </summary>
[Table(TableNames.GenericItem.GenericItemFields, Schema = SchemaNames.GenericItem)]
[PrimaryKey(nameof(Id))]
public class GenericItemField
{
    /// <summary>
    ///     Internal ID for the ItemField
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
    ///   The most recent version of this field
    /// </summary>
    [NotMapped]
    public GenericItemFieldVersion? CurrentVersion => Versions.OrderByDescending(v => v.Version).FirstOrDefault();

    /// <summary>
    ///   The versions for this Item field
    /// </summary>
    public List<GenericItemFieldVersion> Versions { get; set; } = [];

    /// <summary>
    ///   Disables creating of new layouts with this field,
    ///   and when all items using it are deleted it will be removed.
    /// </summary>
    public bool DeleteFlag { get; set; } = false;

    internal static void BuildTable(EntityTypeBuilder<GenericItemField> builder)
    {
        // Remove deleted items from being included in default queries
        builder.HasQueryFilter(x => !x.DeleteFlag);
    }
}