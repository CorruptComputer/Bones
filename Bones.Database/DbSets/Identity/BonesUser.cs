using Microsoft.AspNetCore.Identity;

namespace Bones.Database.DbSets.Identity;

/// <summary>
///     Model for the Identity.BonesUsers table.
/// </summary>
[Table("BonesUsers", Schema = "Identity")]
public class BonesUser : IdentityUser<Guid>
{
    /// <summary>
    ///     When the account was created.
    /// </summary>
    public DateTimeOffset CreateDateTime { get; init; }

    /// <summary>
    ///     When was it confirmed, if at all?
    /// </summary>
    public DateTimeOffset? EmailConfirmedDateTime { get; set; }
}