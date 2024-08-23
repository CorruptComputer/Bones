namespace Bones.Database.DbSets.AssetManagement;

/// <summary>
///     Model for the AssetManagement.AssetLayoutVersions table
/// </summary>
[Table("AssetLayoutVersions", Schema = "AssetManagement")]
[PrimaryKey(nameof(Id))]
public class AssetLayoutVersion
{
    /// <summary>
    ///     Internal ID for the AssetLayoutVersion
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; init; }

    /// <summary>
    ///   The layout this version belongs to
    /// </summary>
    public required AssetLayout AssetLayout { get; init; }

    /// <summary>
    ///   The version number for this layout
    /// </summary>
    public required long Version { get; init; }

    public List<AssetField> Fields { get; init; } = [];

    /// <summary>
    ///   Disables creating of new items using this layout version,
    ///   and when all items using it are deleted it will be removed.
    /// </summary>
    public bool DeleteFlag { get; set; } = false;
}