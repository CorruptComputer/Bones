using System.ComponentModel.DataAnnotations.Schema;
using Bones.Database.DbSets.Identity;
using Microsoft.EntityFrameworkCore;

namespace Bones.Database.DbSets;

/// <summary>
///     Model for the AccountEmailVerification table.
/// </summary>
[Table("AccountEmailVerification")]
[PrimaryKey(nameof(Id))]
public class AccountEmailVerification
{
    /// <summary>
    ///     Internal ID for the Account Email Verification.
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    ///     Internal ID of the account this verification is for.
    /// </summary>
    [ForeignKey(nameof(Account))]
    public required Guid AccountId { get; set; }

    /// <summary>
    ///     Verification token.
    /// </summary>
    // TODO: Might want to use something other than a Guid for this at some point
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