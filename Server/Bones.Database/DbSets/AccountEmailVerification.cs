using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Bones.Database.DbSets;

/// <summary>
///     Model for the AccountEmailVerification table.
/// </summary>
[Table("AccountEmailVerification")]
[PrimaryKey(nameof(EmailVerificationId))]
public class AccountEmailVerification
{
    /// <summary>
    ///     Internal ID for the Account Email Verification.
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long EmailVerificationId { get; set; }

    /// <summary>
    ///     Internal ID of the account this verification is for.
    /// </summary>
    public required long AccountId { get; set; }

    /// <summary>
    ///     Verification token.
    /// </summary>
    public required Guid Token { get; set; }

    /// <summary>
    ///     Time this verification request was created.
    /// </summary>
    public required DateTimeOffset CreateDateTime { get; set; }

    /// <summary>
    ///     Time this verification is valid until.
    /// </summary>
    public required DateTimeOffset ValidUntilDateTime { get; set; }
}