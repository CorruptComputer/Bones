using Microsoft.AspNetCore.Identity;

namespace Bones.Database.DbSets.Identity;

/// <summary>
///     Model for the Identity.BonesRoleClaims table.
/// </summary>
[Table("BonesRoleClaims", Schema = "Identity")]
public class BonesRoleClaim : IdentityRoleClaim<Guid>
{
}