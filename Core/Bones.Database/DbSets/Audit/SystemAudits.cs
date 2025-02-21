using Bones.Database.DbConsts;
using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.System;

namespace Bones.Database.DbSets.Audit;

/// <summary>
///     Model for the Audit.SystemAudits table
/// </summary>
[Table(TableNames.Audit.SystemAudits, Schema = SchemaNames.Audit)]
[PrimaryKey(nameof(Id))]
public class SystemAudit
{
    /// <summary>
    ///     Internal ID for the audit
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
    ///   The entity that took the action
    /// </summary>
    public required BonesUser ActionTakenBy { get; init; }

    /// <summary>
    ///   The reason the action was taken
    /// </summary>
    [MaxLength(512)]
    public required string Reason { get; init; }

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