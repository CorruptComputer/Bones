using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.OrganizationManagement;
using Bones.Shared.Backend.Enums;

namespace Bones.Database.DbSets.AssetManagement.Assets;

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
    ///   The type of owner this asset has
    /// </summary>

    public OwnershipType OwnerType { get; set; }

    /// <summary>
    ///   If the OwnerType is a User, the user that owns this asset
    /// </summary>
    public BonesUser? OwningUser { get; set; }

    /// <summary>
    ///   If the OwnerType is an Organization, the organization that owns this asset
    /// </summary>
    public BonesOrganization? OwningOrganization { get; set; }

    /// <summary>
    ///   The versions for this asset
    /// </summary>
    public List<AssetVersion> Versions { get; set; } = [];

    /// <summary>
    ///   Disables access to this Project and schedules deletes for everything within,
    ///   when all items using it are deleted it will be removed.
    /// </summary>
    public bool DeleteFlag { get; set; } = false;
}