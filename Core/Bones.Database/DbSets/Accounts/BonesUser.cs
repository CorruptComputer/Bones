using Bones.Database.DbConsts;
using Bones.Database.DbSets.Projects;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bones.Database.DbSets.Accounts;

/// <summary>
///   Model for the Accounts.BonesUsers table.
/// </summary>
[Table(TableNames.Accounts.BonesUsers, Schema = SchemaNames.Accounts)]
public class BonesUser : IdentityUser<Guid>
{
    /// <summary>
    ///   Display name for the user, if this is not set it should display their email.
    /// </summary>
    [MaxLength(256)]
    public string? DisplayName { get; set; }

    /// <summary>
    ///   When the account was created.
    /// </summary>
    public DateTimeOffset CreateDateTime { get; init; } = DateTimeOffset.Now;

    /// <summary>
    ///   When was it confirmed, if at all?
    /// </summary>
    public DateTimeOffset? EmailConfirmedDateTime { get; set; }

    /// <summary>
    ///   When was the password last set?
    /// </summary>
    public DateTimeOffset PasswordLastSetDateTime { get; set; } = DateTimeOffset.Now;

    /// <summary>
    ///   If their password is expired, we don't want to allow them to login.
    /// </summary>
    public bool PasswordExpired { get; set; } = false;

    #region Navigational Properties
    /// <summary>
    ///   Navigational property for the projects that the user owns, empty if not .Include()'d in the query
    /// </summary>
    public ICollection<Project> Projects { get; set; } = [];
    #endregion

    /// <summary>
    ///   Needed to override the default table name and schema that <see cref="IdentityUser{Guid}" /> uses.
    ///   Seems the attribute is ignored by that, still keeping it there for consistency though.
    /// </summary>
    /// <param name="builder"></param>
    internal static void BuildTable(EntityTypeBuilder<BonesUser> builder)
    {
        builder.ToTable(TableNames.Accounts.BonesUsers, SchemaNames.Accounts);

        builder.HasMany(u => u.Projects)
               .WithOne(p => p.OwningUser)
               .HasForeignKey(p => p.OwningUserId);
    }
}