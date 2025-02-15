using Bones.Database.DbConsts;
using Bones.Shared.Backend.Enums;

namespace Bones.Database.DbSets.GenericItems;

/// <summary>
///     Model for the GenericItems.GenericItemLayoutVersions table
/// </summary>
[Table(TableNames.GenericItem.GenericItemLayoutVersions, Schema = SchemaNames.GenericItem)]
[PrimaryKey(nameof(Id))]
public class GenericItemLayoutVersion
{
    /// <summary>
    ///     Internal ID for the ItemLayoutVersion
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; init; }

    /// <summary>
    ///   The name for this Item layout
    /// </summary>
    [MaxLength(512)]
    public required string Name { get; set; }
    
    /// <summary>
    ///   The uses this layout is applicable to
    /// </summary>
    public required ItemLayoutUses EnabledFor { get; set; }

    /// <summary>
    ///   The date and time this layout version was created
    /// </summary>
    public DateTimeOffset CreateDateTime { get; init; } = DateTimeOffset.Now;

    /// <summary>
    ///   The ID of the layout this version belongs to
    /// </summary>
    [ForeignKey(nameof(GenericItemLayout))]
    public required Guid ItemLayoutId { get; init; }

    /// <summary>
    ///   The version number for this
    /// </summary>
    public required long Version { get; init; }

    /// <summary>
    ///   The field versions associated with this layout version
    /// </summary>
    public List<GenericItemFieldVersion> Fields { get; init; } = [];

    /// <summary>
    ///   Disables creating of new items using this layout version,
    ///   and when all items using it are deleted it will be removed.
    /// </summary>
    public bool DeleteFlag { get; set; } = false;
}