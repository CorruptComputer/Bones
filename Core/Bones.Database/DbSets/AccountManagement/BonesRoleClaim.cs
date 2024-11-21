using Microsoft.AspNetCore.Identity;

namespace Bones.Database.DbSets.AccountManagement;

/// <summary>
///     Model for the AccountManagement.BonesRoleClaims table.
/// </summary>
[Table("BonesRoleClaims", Schema = "AccountManagement")]
public class BonesRoleClaim : IdentityRoleClaim<Guid>;