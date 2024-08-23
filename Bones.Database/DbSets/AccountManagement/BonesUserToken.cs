using Microsoft.AspNetCore.Identity;

namespace Bones.Database.DbSets.AccountManagement;

/// <summary>
///     Model for the AccountManagement.BonesUserTokens table.
/// </summary>
[Table("BonesUserTokens", Schema = "AccountManagement")]
public class BonesUserToken : IdentityUserToken<Guid>;