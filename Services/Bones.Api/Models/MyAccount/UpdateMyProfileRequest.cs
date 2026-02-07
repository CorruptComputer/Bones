using Bones.Database.DbSets.Accounts;
using Bones.Logic.Features.Accounts;

namespace Bones.Api.Models.MyAccount;


/// <summary>
///   Request for the UpdateMyProfileAsync endpoint
/// </summary>
[JsonSerializable(typeof(GetMyProfileResponse))]
public record UpdateMyProfileRequest
{
    /// <summary>
    ///   The display name to set for the user
    /// </summary>
    [JsonRequired]
    public required string DisplayName { get; init; }

    internal UpdateMyProfile.Command ToInternal(BonesUser user)
    {
        return new(DisplayName, user);
    }
}
