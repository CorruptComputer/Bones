using System.Net;
using Bones.Database.DbConsts;
using Bones.Database.DbSets.Accounts;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bones.Database.DbSets.Audits;

/// <summary>
///   Model for the Audits.LoginAudits table
/// </summary>
[Table(TableNames.Audits.LoginAudits, Schema = SchemaNames.Audits)]
[PrimaryKey(nameof(Id))]
public class LoginAudit
{
    /// <summary>
    ///   Internal ID for the audit
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; init; }

    /// <summary>
    ///   The ID of the user account that was attempted to be logged into, if known
    /// </summary>
    public Guid? BonesUserId { get; init; }

    /// <summary>
    ///   If the account is unknown, the email address that was used in the login attempt
    /// </summary>
    public string? UnknownEmail { get; init; }

    /// <summary>
    ///   The date and time the the login was attempted
    /// </summary>
    public required DateTimeOffset LoginDateTime { get; init; }

    /// <summary>
    ///   Was the login attempt successful?
    /// </summary>
    public required bool Successful { get; init; }

    /// <summary>
    ///   The IP address that the login was attempted from
    /// </summary>
    public required IPAddress RequestingIpAddress { get; init; }

    // Ideally I'd also like to add a bool for if the password used has been flagged by HaveIBeenPwned
    // https://haveibeenpwned.com/API/v3#PwnedPasswords
    // https://github.com/IEvangelist/pwned-client?tab=readme-ov-file#dependency-injection

    #region Navigational Properties
    /// <summary>
    ///   Navigational property for the account this audit entry belongs to, null if not .Include()'d in the query or unknown
    /// </summary>
    public BonesUser? BonesUser { get; set; }
    #endregion

    internal static void BuildTable(EntityTypeBuilder<LoginAudit> builder)
    {
        builder.HasOne(la => la.BonesUser)
               .WithMany()
               .HasForeignKey(la => la.BonesUserId);
    }
}