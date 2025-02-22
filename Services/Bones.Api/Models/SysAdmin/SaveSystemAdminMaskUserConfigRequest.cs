using Bones.Database.DbSets.AccountManagement;
using Bones.Logic.Features.System.SystemSettings;

namespace Bones.Api.Models.SysAdmin;

/// <summary>
///   Request for the SaveSystemAdminMaskUserConfigAsync endpoint
/// </summary>
[JsonSerializable(typeof(SaveSystemAdminMaskUserConfigRequest))]
public record SaveSystemAdminMaskUserConfigRequest
{
    /// <summary>
    ///   Should the system admin mask user be enabled?
    /// </summary>
    public required bool IsEnabled { get; init; }

    /// <summary>
    ///   The email of the system admin mask user
    /// </summary>
    public required string Email { get; init; }

    /// <summary>
    ///   The display name of the system admin mask user
    /// </summary>
    public required string DisplayName { get; init; }

    /// <summary>
    ///   The reason for the change
    /// </summary>
    public required string ChangeReason { get; init; }

    internal SaveSystemAdminMaskUserConfig.Command ToInternal(BonesUser user)
    {
        return new(IsEnabled, Email, DisplayName, ChangeReason, user);
    }
}
