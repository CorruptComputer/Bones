using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.OrganizationManagement;
using Bones.Shared.Backend.Enums;

namespace Bones.Database.DbSets.AssetManagement.AssetLayouts;

/// <summary>
///     Model for the AssetManagement.AssetLayouts table
/// </summary>
[Table("AssetLayouts", Schema = "AssetManagement")]
[PrimaryKey(nameof(Id))]
public class AssetLayout
{
    /// <summary>
    ///     Internal ID for the AssetLayout
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; init; }

    /// <summary>
    ///   The name for this asset layout
    /// </summary>
    [MaxLength(512)]
    public required string Name { get; set; }
    
    /// <summary>
    ///   The type of owner this asset layout has
    /// </summary>
    public OwnershipType OwnerType { get; set; }

    /// <summary>
    ///   If the OwnerType is a User, the user that owns this asset layout
    /// </summary>
    public BonesUser? OwningUser { get; set; }

    /// <summary>
    ///   If the OwnerType is an Organization, the organization that owns this asset layout
    /// </summary>
    public BonesOrganization? OwningOrganization { get; set; }

    /// <summary>
    ///   The versions for this asset layout
    /// </summary>
    public List<AssetLayoutVersion> Versions { get; set; } = [];

    /// <summary>
    ///   Disables creating of new items using this layout,
    ///   and when all items using it are deleted it will be removed.
    /// </summary>
    public bool DeleteFlag { get; set; } = false;
}