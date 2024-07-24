using Microsoft.AspNetCore.Identity;

namespace Bones.Database.DbSets.Identity;

/// <summary>
///     Model for the Identity.BonesUserRoles table.
/// </summary>
[Table("BonesUserRoles", Schema = "Identity")]
public class BonesUserRole : IdentityUserRole<Guid>
{
}