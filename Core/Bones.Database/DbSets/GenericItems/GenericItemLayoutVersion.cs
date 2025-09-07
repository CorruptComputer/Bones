using Bones.Database.DbConsts;
using Bones.Shared.Backend.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bones.Database.DbSets.GenericItems;

/// <summary>
///   Model for the GenericItems.GenericItemLayoutVersions table
/// </summary>
[Table(TableNames.GenericItem.GenericItemLayoutVersions, Schema = SchemaNames.GenericItem)]
[PrimaryKey(nameof(Id))]
public class GenericItemLayoutVersion
{
    /// <summary>
    ///   Internal ID for the ItemLayoutVersion
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; init; }

    /// <summary>
    ///   The name for this Item layout
    /// </summary>
    [MaxLength(512)]
    public required string Name { get; set; }

    /// <summary>
    ///   The usage this layout is applicable for
    /// </summary>
    public required ItemLayoutUse LayoutUse { get; set; }

    /// <summary>
    ///   The date and time this layout version was created
    /// </summary>
    public DateTimeOffset CreateDateTime { get; init; } = DateTimeOffset.Now;

    /// <summary>
    ///   The ID of the layout this version belongs to
    /// </summary>
    public required GenericItemLayout ItemLayout { get; init; }

    /// <summary>
    ///   The version number for this
    /// </summary>
    public required long Version { get; init; }

    /// <summary>
    ///   The field links associated with this layout version
    /// </summary>
    public List<GenericItemLayoutFieldVersionLink> FieldLinks { get; init; } = [];

    /// <summary>
    ///   Disables creating of new items using this layout version,
    ///   and when all items using it are deleted it will be removed.
    /// </summary>
    public bool DeleteFlag { get; set; } = false;

    internal static void BuildTable(EntityTypeBuilder<GenericItemLayoutVersion> builder)
    {
        // Remove deleted items from being included in default queries
        builder.HasQueryFilter(x => !x.DeleteFlag);
    }
}