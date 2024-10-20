using Bones.Database.DbSets.ProjectManagement;
using Microsoft.AspNetCore.Identity;

namespace Bones.Database.DbSets.AccountManagement;

/// <summary>
///     Model for the AccountManagement.BonesUsers table.
/// </summary>
[Table("BonesUsers", Schema = "AccountManagement")]
public class BonesUser : IdentityUser<Guid>
{
    /// <summary>
    ///   Display name for the user, if this is not set it should display their email.
    /// </summary>
    [MaxLength(256)]
    public string? DisplayName { get; set; }

    /// <summary>
    ///     When the account was created.
    /// </summary>
    [Required]
    public DateTimeOffset CreateDateTime { get; } = DateTimeOffset.Now;

    /// <summary>
    ///     When was it confirmed, if at all?
    /// </summary>
    public DateTimeOffset? EmailConfirmedDateTime { get; set; }

    /// <summary>
    ///   If their password is expired, we don't want to allow them to login.
    ///   TODO: Make this actually do something
    /// </summary>
    public bool PasswordExpired { get; set; } = false;

    /// <summary>
    ///   The projects that the user owns.
    /// </summary>
    public List<Project> Projects { get; set; } = [];
}