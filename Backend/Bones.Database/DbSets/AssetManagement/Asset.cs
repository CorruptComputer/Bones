using Bones.Database.DbSets.GenericItems.Items;
using Bones.Database.DbSets.ProjectManagement;

namespace Bones.Database.DbSets.AssetManagement;

/// <summary>
///     Model for the AssetManagement.Assets table
/// </summary>
[Table("Assets", Schema = "AssetManagement")]
[PrimaryKey(nameof(Id))]
public class Asset
{
    /// <summary>
    ///     Internal ID for the Tag
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    ///   The name of this asset
    /// </summary>
    [MaxLength(512)]
    public required string Name { get; set; }

    /// <summary>
    ///   The project this Asset belongs to
    /// </summary>
    public required Project Project { get; set; }

    /// <summary>
    ///   The generic item for this asset
    /// </summary>
    public required Item Item { get; set; }

    /// <summary>
    ///   Disables access to this Asset and schedules deletes for everything within,
    ///   when all items using it are deleted it will be removed.
    /// </summary>
    public bool DeleteFlag { get; set; } = false;
}