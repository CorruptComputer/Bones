using Microsoft.AspNetCore.Identity;

namespace Bones.Database.DbSets.Identity;

/// <summary>
///     Model for the Identity.BonesRoles table.
/// </summary>
[Table("BonesRoles", Schema = "Identity")]
public class BonesRole : IdentityRole<Guid>
{
}