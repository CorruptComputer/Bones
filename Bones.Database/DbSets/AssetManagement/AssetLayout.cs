namespace Bones.Database.DbSets.AssetManagement;

/// <summary>
///     Model for the AssetManagement.AssetLayouts table
/// </summary>
[Table("AssetLayouts", Schema = "AssetManagement")]
[PrimaryKey(nameof(Id))]
public class AssetLayout
{
    /// <summary>
    ///     Internal ID for the AssetLayout Version
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; init; }

    [MaxLength(512)]
    public required string Name { get; set; }

    /// <summary>
    ///   The versions for this layout
    /// </summary>
    public List<AssetLayoutVersion> Versions { get; set; } = [];

    /// <summary>
    ///   Disables creating of new items using this layout,
    ///   and when all items using it are deleted it will be removed.
    /// </summary>
    public bool DeleteFlag { get; set; } = false;
}