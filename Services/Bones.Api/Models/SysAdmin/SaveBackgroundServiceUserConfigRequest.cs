using Bones.Database.DbSets.AccountManagement;
using Bones.Logic.Features.System.SystemSettings;

namespace Bones.Api.Models.SysAdmin;

/// <summary>
///   Request for the SaveBackgroundServiceUserConfigAsync endpoint
/// </summary>
[JsonSerializable(typeof(SaveBackgroundServiceUserConfigRequest))]
public record SaveBackgroundServiceUserConfigRequest
{
    /// <summary>
    ///   The email of the background service user
    /// </summary>
    public required string Email { get; init; }

    /// <summary>
    ///   The display name of the background service user
    /// </summary>
    public required string DisplayName { get; init; }

    /// <summary>
    ///   The reason for the change
    /// </summary>
    public required string ChangeReason { get; init; }

    internal SaveBackgroundServiceUserConfig.Command ToInternal(BonesUser user)
    {
        return new(Email, DisplayName, ChangeReason, user);
    }
}
