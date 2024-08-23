using Bones.Database.DbSets.OrganizationManagement;
using Microsoft.AspNetCore.Identity;

namespace Bones.Database.DbSets.AccountManagement;

/// <summary>
///     Model for the AccountManagement.BonesRoles table.
/// </summary>
[Table("BonesRoles", Schema = "AccountManagement")]
public class BonesRole : IdentityRole<Guid>
{
    public bool IsSystemRole { get; set; } = false;
    
    public BonesOrganization? Organization { get; set; }
}