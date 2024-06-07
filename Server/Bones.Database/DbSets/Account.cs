using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Bones.Database.DbSets;

/// <summary>
///     Model for the Account table.
/// </summary>
[Table("Account")]
[PrimaryKey(nameof(AccountId))]
public class Account
{
    /// <summary>
    ///     Internal ID for the Account.
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long AccountId { get; set; }

    /// <summary>
    ///     When the account was created.
    /// </summary>
    public required DateTimeOffset CreateDateTime { get; set; }

    /// <summary>
    ///     Current email address for the account.
    /// </summary>
    [EmailAddress]
    public required string Email { get; set; }

    /// <summary>
    ///     Is the current email verified?
    /// </summary>
    public required bool EmailVerified { get; set; }

    /// <summary>
    ///     If it is verified, when was it verified.
    /// </summary>
    public DateTimeOffset? EmailVerifiedDateTime { get; set; }

    /// <summary>
    ///     Previous email for the account
    /// </summary>
    [EmailAddress]
    public string? OldEmail { get; set; }
}