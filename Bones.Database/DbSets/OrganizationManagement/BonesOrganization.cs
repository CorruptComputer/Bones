using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.AssetManagement;
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

    [MaxLength(512)]
    public required string Name { get; set; }
    
    public required List<BonesRole> Roles { get; set; }
    
    public required List<Project> Projects { get; set; }
    
    public required List<Asset> Assets { get; set; }
}