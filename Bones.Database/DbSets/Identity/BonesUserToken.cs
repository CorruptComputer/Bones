using Microsoft.AspNetCore.Identity;

namespace Bones.Database.DbSets.Identity;

/// <summary>
///     Model for the Identity.BonesUserTokens table.
/// </summary>
[Table("BonesUserTokens", Schema = "Identity")]
public class BonesUserToken : IdentityUserToken<Guid>;