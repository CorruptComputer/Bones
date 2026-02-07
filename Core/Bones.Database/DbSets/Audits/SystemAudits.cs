using Bones.Database.DbConsts;
using Bones.Database.DbSets.Accounts;
using Bones.Database.DbSets.System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bones.Database.DbSets.Audits;

/// <summary>
///   Model for the Audits.SystemAudits table
/// </summary>
[Table(TableNames.Audits.SystemAudits, Schema = SchemaNames.Audits)]
[PrimaryKey(nameof(Id))]
public class SystemAudit
{
    /// <summary>
    ///   Internal ID for the audit
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; init; }

    /// <summary>
    ///   The date and time the action was taken
    /// </summary>
    public required DateTimeOffset ActionDateTime { get; init; }

    /// <summary>
    ///   The action that was taken
    /// </summary>
    public required Actions ActionTaken { get; init; }

    /// <summary>
    ///   If the action was a setting update, the setting that was changed
    /// </summary>
    public SystemSetting.SettingType? SettingChanged { get; init; }

    /// <summary>
    ///   The ID of the user that took the action
    /// </summary>
    public required Guid ActionTakenByUserId { get; init; }

    /// <summary>
    ///   The reason the action was taken
    /// </summary>
    [MaxLength(512)]
    public required string Reason { get; init; }

    #region Navigational Properties
    /// <summary>
    ///   Navigational property for the account this audit entry belongs to, null if not .Include()'d in the query
    /// </summary>
    public BonesUser? ActionTakenByUser { get; set; }
    #endregion

    internal static void BuildTable(EntityTypeBuilder<SystemAudit> builder)
    {
        builder.HasOne(sa => sa.ActionTakenByUser)
               .WithMany()
               .HasForeignKey(sa => sa.ActionTakenByUserId);
    }

    /// <summary>
    ///   The actions that can be taken
    /// </summary>
    public enum Actions : ushort
    {
        /// <summary>
        ///   No action taken
        /// </summary>
        SystemSettingUpdate = 1,
    }
}