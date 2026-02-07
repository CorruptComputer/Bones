using Bones.Database.DbConsts;
using Bones.Database.DbSets.Accounts;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bones.Database.DbSets.Audits;

/// <summary>
///   Model for the Audits.AccountAudits table
/// </summary>
[Table(TableNames.Audits.AccountAudits, Schema = SchemaNames.Audits)]
[PrimaryKey(nameof(Id))]
public class AccountAudit
{
    /// <summary>
    ///   Internal ID for the audit
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; init; }

    /// <summary>
    ///   The ID of the account that was acted upon
    /// </summary>
    public required Guid AccountBonesUserId { get; init; }

    /// <summary>
    ///   The date and time the action was taken
    /// </summary>
    public required DateTimeOffset ActionDateTime { get; init; }

    /// <summary>
    ///   The action that was taken on the account
    /// </summary>
    public required Actions ActionTaken { get; init; }

    /// <summary>
    ///   The ID of the account that performed the action
    /// </summary>
    public required Guid ActionTakenByBonesUserId { get; init; }

    /// <summary>
    ///   The reason the action was taken
    /// </summary>
    [MaxLength(512)]
    public required string Reason { get; init; }

    #region Navigational Properties
    /// <summary>
    ///   Navigational property for the account this audit entry belongs to, null if not .Include()'d in the query
    /// </summary>
    public BonesUser? AccountBonesUser { get; set; }

    /// <summary>
    ///   Navigational property for the account that performed the action, null if not .Include()'d in the query
    /// </summary>
    public BonesUser? ActionTakenByBonesUser { get; set; }
    #endregion

    internal static void BuildTable(EntityTypeBuilder<AccountAudit> builder)
    {
        builder.HasOne(aa => aa.AccountBonesUser)
               .WithMany()
               .HasForeignKey(aa => aa.AccountBonesUserId);

        builder.HasOne(aa => aa.ActionTakenByBonesUser)
               .WithMany()
               .HasForeignKey(aa => aa.ActionTakenByBonesUserId);
    }

    /// <summary>
    ///   The actions that can be taken on an account
    /// </summary>
    public enum Actions : ushort
    {
        /// <summary>
        ///   The account was created
        /// </summary>
        Create = 1,

        /// <summary>
        ///   The accounts profile was updated
        /// </summary>
        UpdateProfile = 2,

        /// <summary>
        ///   The accounts email was updated
        /// </summary>
        UpdateEmail = 3,

        /// <summary>
        ///   The accounts password was updated
        /// </summary>
        UpdatePassword = 4,
    }
}
