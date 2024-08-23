using Microsoft.AspNetCore.Identity;

namespace Bones.Database.DbSets.AccountManagement;

/// <summary>
///     Model for the AccountManagement.BonesUserClaims table.
/// </summary>
[Table("BonesUserClaims", Schema = "AccountManagement")]
public class BonesUserClaim : IdentityUserClaim<Guid>;