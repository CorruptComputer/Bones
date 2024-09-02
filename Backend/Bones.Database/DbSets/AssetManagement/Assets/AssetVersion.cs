using Bones.Database.DbSets.AssetManagement.AssetLayouts;

namespace Bones.Database.DbSets.AssetManagement.Assets;

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

    /// <summary>
    ///   The layout version this asset version uses
    /// </summary>
    public required AssetLayoutVersion AssetLayout { get; set; }

    /// <summary>
    ///   The version of this asset
    /// </summary>
    public required long Version { get; set; }

    /// <summary>
    ///   The asset this version belongs to
    /// </summary>
    public required Asset Asset { get; set; }

    /// <summary>
    ///   The values for the fields defined by this asset layout version
    /// </summary>
    public required List<AssetValue> Values { get; set; }

    /// <summary>
    ///   Disables viewing this item version, and when safe to do so it will be removed.
    /// </summary>
    public bool DeleteFlag { get; set; } = false;
}