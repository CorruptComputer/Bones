using System.Runtime.InteropServices;
using Bones.Database.DbConsts;

namespace Bones.Database.DbSets.System;

/// <summary>
///   Model for the System.SystemSettings table
/// </summary>
[Table(TableNames.System.SystemSettings, Schema = SchemaNames.System)]
[PrimaryKey(nameof(Setting))]
[Index(nameof(Setting), IsUnique = true)]
public class SystemSetting
{
    /// <summary>
    ///   ID of the setting
    /// </summary>
    public required SettingType Setting { get; init; }

    /// <summary>
    ///   The value for the setting.
    ///
    ///   Basic data types such as ints, bools, etc, should simply be .ToString()'d
    ///   More complex types should be serialized to JSON
    /// </summary>
    public string? Value { get; set; }

    /// <summary>
    ///   The exact setting, also used as the value for the primary key
    /// </summary>
    public enum SettingType : ushort
    {
        /// <summary>
        ///   The ID of the user that actions taken by the background service should be attributed to
        /// </summary>
        BackgroundServiceUserId = 1,

        /// <summary>
        ///   Should the system admin mask user be enabled?
        /// </summary>
        SystemAdminMaskUserEnabled = 2,

        /// <summary>
        ///   The ID of the user that actions taken by system admins should be attributed to
        /// </summary>
        SystemAdminMaskUserId = 3,

        /// <summary>
        ///   The base URL for the web UI, used for generating links in the background service
        /// </summary>
        WebUIBaseUrl = 4,

        /// <summary>
        ///   Is SMTP enabled?
        /// </summary>
        SmtpEnabled = 5,

        /// <summary>
        ///   The SMTP configuration
        /// </summary>
        SmtpConfig = 6,
    }
}
