using Bones.Database.DbSets.Accounts;

namespace Bones.Api.Models.Accounts;

/// <summary>
///   Response for the GetPublicProfileAsync endpoint
/// </summary>
[JsonSerializable(typeof(GetPublicProfileResponse))]
public sealed record GetPublicProfileResponse
{
    /// <summary>
    ///   The ID of the user
    /// </summary>
    [JsonRequired]
    public required Guid BonesUserId { get; init; }

    /// <summary>
    ///   The display name of the user, defaults to their email if they don't have one set
    /// </summary>
    [JsonRequired]
    public required string DisplayName { get; init; }

    /// <summary>
    ///   The date and time the user was created
    /// </summary>
    [JsonRequired]
    public required DateTimeOffset CreateDateTime { get; init; }

    internal static GetPublicProfileResponse FromUser(BonesUser user)
    {
        return new()
        {
            BonesUserId = user.Id,
            DisplayName = user.DisplayName ?? "Unknown",
            CreateDateTime = user.CreateDateTime,
        };
    }
}

