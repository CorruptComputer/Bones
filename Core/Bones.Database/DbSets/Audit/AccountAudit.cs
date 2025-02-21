using Bones.Database.DbConsts;
using Bones.Database.DbSets.AccountManagement;

namespace Bones.Database.DbSets.Audit;

/// <summary>
///     Model for the Audit.AccountAudits table
/// </summary>
[Table(TableNames.Audit.AccountAudits, Schema = SchemaNames.Audit)]
[PrimaryKey(nameof(Id))]
public class AccountAudit
{
    /// <summary>
    ///     Internal ID for the audit
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; init; }

    /// <summary>
    ///   The user account that was acted upon
    /// </summary>
    public required BonesUser Account { get; init; }

    /// <summary>
    ///   The date and time the action was taken
    /// </summary>
    public required DateTimeOffset ActionDateTime { get; init; }

    /// <summary>
    ///   The action that was taken on the account
    /// </summary>
    public required Actions ActionTaken { get; init; }

    /// <summary>
    ///   The entity that took the action
    /// </summary>
    public required BonesUser ActionTakenBy { get; init; }

    /// <summary>
    ///   The reason the action was taken
    /// </summary>
    [MaxLength(512)]
    public required string Reason { get; init; }

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
    }
}
