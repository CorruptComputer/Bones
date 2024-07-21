using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Bones.Database.DbSets.Identity;

/// <summary>
///     Model for the Identity.AccountLogins table.
/// </summary>
[Table("AccountLogins", Schema = "Identity")]
public class AccountLogin : IdentityUserLogin<Guid>
{

}