using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.OrganizationManagement;
using Bones.Shared.Backend.Enums;

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

    [MaxLength(512)]
    public required string Name { get; set; }
    
    public OwnershipType OwnerType { get; set; }
    
    public BonesUser? OwningUser { get; set; }
    
    public BonesOrganization? OwningOrganization { get; set; }

    /// <summary>
    ///   Disables access to this Project and schedules deletes for everything within,
    ///   when all items using it are deleted it will be removed.
    /// </summary>
    public bool DeleteFlag { get; set; } = false;
}