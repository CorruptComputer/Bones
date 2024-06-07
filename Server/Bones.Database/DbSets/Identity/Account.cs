using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Bones.Database.DbSets.Identity;

/// <summary>
///     Model for the Identity.Accounts table.
/// </summary>
[Table("Accounts", Schema = "Identity")]
public class Account : IdentityUser<Guid>
{
    /// <summary>
    ///     When the account was created.
    /// </summary>
    public DateTimeOffset CreateDateTime { get; set; }

    /// <summary>
    ///     When was it confirmed, if at all?
    /// </summary>
    public DateTimeOffset? EmailConfirmedDateTime { get; set; }
}