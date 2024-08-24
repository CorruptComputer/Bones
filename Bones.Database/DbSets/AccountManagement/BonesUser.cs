using Bones.Database.DbSets.AssetManagement;
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
    ///     When the account was created.
    /// </summary>
    public DateTimeOffset CreateDateTime { get; init; }

    /// <summary>
    ///     When was it confirmed, if at all?
    /// </summary>
    public DateTimeOffset? EmailConfirmedDateTime { get; set; }

    public bool PasswordExpired { get; set; } = false;

    public List<Project> Projects { get; set; } = [];

    public List<Asset> Assets { get; set; } = [];
}