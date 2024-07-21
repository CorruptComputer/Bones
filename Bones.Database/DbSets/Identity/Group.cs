using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Bones.Database.DbSets.Identity;

/// <summary>
///     Model for the Identity.Groups table.
/// </summary>
[Table("Groups", Schema = "Identity")]
public class Group : IdentityRole<Guid>
{
}