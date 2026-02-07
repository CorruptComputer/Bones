using Bones.Database.DbSets.Accounts;

namespace Bones.Api.Models.SysAdmin;

/// <summary>
///   Response for the GetBackgroundServiceUserConfigAsync endpoint
/// </summary>
[JsonSerializable(typeof(GetBackgroundServiceUserConfigResponse))]
public record GetBackgroundServiceUserConfigResponse
{
    /// <summary>
    ///   The email of the background service user
    /// </summary>
    public required string Email { get; init; }

    /// <summary>
    ///   The display name of the background service user
    /// </summary>
    public required string DisplayName { get; init; }

    internal static GetBackgroundServiceUserConfigResponse FromInternal(BonesUser user)
    {
        return new()
        {
            Email = user.Email ?? string.Empty,
            DisplayName = user.DisplayName ?? "Unknown"
        };
    }
}
