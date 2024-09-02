using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.AssetManagement.AssetLayouts;
using Bones.Database.DbSets.AssetManagement.Assets;
using Bones.Database.DbSets.ProjectManagement;

namespace Bones.Database.DbSets.OrganizationManagement;

/// <summary>
///     Model for the OrganizationManagement.BonesOrganizations table
/// </summary>
[Table("BonesOrganizations", Schema = "OrganizationManagement")]
[PrimaryKey(nameof(Id))]
public class BonesOrganization
{
    /// <summary>
    ///     Internal ID for the BonesOrganization
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    ///   The name of this organization
    /// </summary>
    [MaxLength(512)]
    public required string Name { get; set; }

    /// <summary>
    ///   The roles associated with this organization
    /// </summary>
    public required List<BonesRole> Roles { get; set; }

    /// <summary>
    ///   The projects associated with this organization
    /// </summary>
    public required List<Project> Projects { get; set; }
    
    /// <summary>
    ///   The assets associated with this organization
    /// </summary>
    public required List<Asset> Assets { get; set; }
    
    /// <summary>
    ///   The asset layouts associated with this organization
    /// </summary>
    public required List<AssetLayout> AssetLayouts { get; set; }
}