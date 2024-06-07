using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Bones.Database.DbSets.Identity;

/// <summary>
///     Model for the Identity.GroupPermissions table.
/// </summary>
[Table("GroupPermissions", Schema = "Identity")]
public class GroupPermission : IdentityRoleClaim<Guid>
{
}