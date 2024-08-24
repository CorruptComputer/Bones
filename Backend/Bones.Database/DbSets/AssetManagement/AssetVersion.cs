namespace Bones.Database.DbSets.AssetManagement;

/// <summary>
///     Model for the AssetManagement.AssetVersions table
/// </summary>
[Table("AssetVersions", Schema = "AssetManagement")]
[PrimaryKey(nameof(Id))]
public class AssetVersion
{
    /// <summary>
    ///     Internal ID for the AssetVersion
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public required AssetLayoutVersion AssetLayout { get; set; }

    public required long Version { get; set; }

    public required Asset Asset { get; set; }

    public required List<AssetValue> Values { get; set; }

    /// <summary>
    ///   Disables viewing this item version, and when safe to do so it will be removed.
    /// </summary>
    public bool DeleteFlag { get; set; } = false;
}