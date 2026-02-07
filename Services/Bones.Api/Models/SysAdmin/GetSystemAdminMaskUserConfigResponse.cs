using Bones.Database.DbSets.Accounts;

namespace Bones.Api.Models.SysAdmin;

/// <summary>
///   Response for the GetSystemAdminMaskUserConfigAsync endpoint
/// </summary>
[JsonSerializable(typeof(GetSystemAdminMaskUserConfigResponse))]
public record GetSystemAdminMaskUserConfigResponse
{
    /// <summary>
    ///   Is the system admin mask user enabled?
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

    internal static GetSystemAdminMaskUserConfigResponse FromInternal(bool enabled, BonesUser? user)
    {
        return new()
        {
            IsEnabled = enabled,
            Email = user?.Email ?? string.Empty,
            DisplayName = user?.DisplayName ?? string.Empty
        };
    }
}
