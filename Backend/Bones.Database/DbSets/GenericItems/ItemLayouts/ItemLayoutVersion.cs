using Bones.Database.DbSets.GenericItems.ItemFields;

namespace Bones.Database.DbSets.GenericItems.ItemLayouts;

/// <summary>
///     Model for the GenericItems.ItemLayoutVersions table
/// </summary>
[Table("ItemLayoutVersions", Schema = "GenericItems")]
[PrimaryKey(nameof(Id))]
public class ItemLayoutVersion
{
    /// <summary>
    ///     Internal ID for the ItemLayoutVersion
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; init; }

    /// <summary>
    ///   The ID of the layout this version belongs to
    /// </summary>
    [ForeignKey(nameof(ItemLayout))]
    public required Guid ItemLayoutId { get; init; }

    /// <summary>
    ///   The version number for this
    /// </summary>
    public required long Version { get; init; }

    /// <summary>
    ///   The time which this was created
    /// </summary>
    public required DateTimeOffset CreateDateTime { get; init; }

    /// <summary>
    ///   The fields associated with this layout version
    /// </summary>
    public List<ItemField> Fields { get; init; } = [];

    /// <summary>
    ///   Disables creating of new items using this layout version,
    ///   and when all items using it are deleted it will be removed.
    /// </summary>
    public bool DeleteFlag { get; set; } = false;
}