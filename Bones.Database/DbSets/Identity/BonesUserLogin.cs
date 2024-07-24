using Microsoft.AspNetCore.Identity;

namespace Bones.Database.DbSets.Identity;

/// <summary>
///     Model for the Identity.BonesUserLogins table.
/// </summary>
[Table("BonesUserLogins", Schema = "Identity")]
public class BonesUserLogin : IdentityUserLogin<Guid>
{

}