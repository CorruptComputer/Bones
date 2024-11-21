using Microsoft.AspNetCore.Identity;

namespace Bones.Database.DbSets.AccountManagement;

/// <summary>
///     Model for the AccountManagement.BonesUserRoles table.
/// </summary>
[Table("BonesUserRoles", Schema = "AccountManagement")]
public class BonesUserRole : IdentityUserRole<Guid>;