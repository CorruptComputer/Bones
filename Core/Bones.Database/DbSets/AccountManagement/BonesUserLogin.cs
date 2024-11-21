using Microsoft.AspNetCore.Identity;

namespace Bones.Database.DbSets.AccountManagement;

/// <summary>
///     Model for the AccountManagement.BonesUserLogins table.
/// </summary>
[Table("BonesUserLogins", Schema = "AccountManagement")]
public class BonesUserLogin : IdentityUserLogin<Guid>;