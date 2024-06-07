using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Bones.Database.DbSets.Identity;

/// <summary>
///     Model for the Identity.AccountTokens table.
/// </summary>
[Table("AccountTokens", Schema = "Identity")]
public class AccountToken : IdentityUserToken<Guid>
{
}