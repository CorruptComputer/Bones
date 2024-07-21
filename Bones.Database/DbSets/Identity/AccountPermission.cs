using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Bones.Database.DbSets.Identity;

/// <summary>
///     Model for the Identity.AccountPermissions table.
/// </summary>
[Table("AccountPermissions", Schema = "Identity")]
public class AccountPermission : IdentityUserClaim<Guid>
{
}