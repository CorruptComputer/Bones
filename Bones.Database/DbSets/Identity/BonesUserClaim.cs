using Microsoft.AspNetCore.Identity;

namespace Bones.Database.DbSets.Identity;

/// <summary>
///     Model for the Identity.BonesUserClaims table.
/// </summary>
[Table("BonesUserClaims", Schema = "Identity")]
public class BonesUserClaim : IdentityUserClaim<Guid>;