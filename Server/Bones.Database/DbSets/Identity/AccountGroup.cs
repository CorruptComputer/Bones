using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Bones.Database.DbSets.Identity;

/// <summary>
///     Model for the Identity.AccountGroups table.
/// </summary>
[Table("AccountGroups", Schema = "Identity")]
public class AccountGroup : IdentityUserRole<Guid>
{
}