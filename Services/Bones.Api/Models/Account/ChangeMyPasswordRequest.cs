using Bones.Database.DbSets.AccountManagement;
using Bones.Logic.Features.Accounts;

namespace Bones.Api.Models.Account;

/// <summary>
///   Request for the ChangeMyPasswordAsync endpoint
/// </summary>
[JsonSerializable(typeof(ChangeMyPasswordRequest))]
public record ChangeMyPasswordRequest
{
    /// <summary>
    ///   The new password for the user
    /// </summary>
    [JsonRequired]
    public required string NewPassword { get; init; }

    /// <summary>
    ///   The current password for the user
    /// </summary>
    [JsonRequired]
    public required string CurrentPassword { get; init; }

    /// <summary>
    ///   Should sessions other than the current one be invalidated?
    /// </summary>
    [JsonRequired]
    public required bool InvalidateOtherSessions { get; init; }

    /// <summary>
    ///   If InvalidateOtherSessions is true, this should be the ID of the current session.
    /// </summary>
    public Guid? CurrentSessionId { get; init; }

    internal ChangePassword.Command ToInternal(BonesUser user)
    {
        return new(CurrentPassword, NewPassword, InvalidateOtherSessions, CurrentSessionId, user);
    }
}
