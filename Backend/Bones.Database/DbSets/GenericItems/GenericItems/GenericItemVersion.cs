using Bones.Database.DbSets.GenericItems.GenericItemLayouts;

namespace Bones.Database.DbSets.GenericItems.GenericItems;

/// <summary>
///     Model for the GenericItems.GenericItemVersions table
/// </summary>
[Table("GenericItemVersions", Schema = "GenericItems")]
[PrimaryKey(nameof(Id))]
public class GenericItemVersion
{
    /// <summary>
    ///     Internal ID for the ItemVersion
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    ///   The ID of the Item this version belongs to
    /// </summary>
    [ForeignKey(nameof(GenericItem))]
    public required Guid ItemId { get; init; }

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
    public required GenericItemLayoutVersion GenericItemLayoutVersion { get; set; }

    /// <summary>
    ///   The values for the fields defined by this items layout version
    /// </summary>
    public required List<GenericItemValue> Values { get; set; }

    /// <summary>
    ///   Disables viewing this item version, and when safe to do so it will be deleted.
    /// </summary>
    public bool DeleteFlag { get; set; } = false;
}