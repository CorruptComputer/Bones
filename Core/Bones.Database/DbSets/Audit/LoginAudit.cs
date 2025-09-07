using System.Net;
using Bones.Database.DbConsts;
using Bones.Database.DbSets.AccountManagement;

namespace Bones.Database.DbSets.Audit;

/// <summary>
///   Model for the Audit.LoginAudits table
/// </summary>
[Table(TableNames.Audit.LoginAudits, Schema = SchemaNames.Audit)]
[PrimaryKey(nameof(Id))]
public class LoginAudit
{
    /// <summary>
    ///   Internal ID for the audit
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; init; }

    /// <summary>
    ///   The user account that was attempted to be logged into, if known
    /// </summary>
    public BonesUser? Account { get; init; }

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
}