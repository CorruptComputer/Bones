using System.Text.Json.Serialization;

namespace Bones.Api.Features.Auth.GetUserInfo;

[JsonSerializable(typeof(GetUserInfoResponse))]
public sealed record GetUserInfoResponse
{
    /// <summary>
    ///   The email address associated with the authenticated user.
    /// </summary>
    public required string Email { get; init; }

    /// <summary>
    ///   Indicates whether the <see cref="P:Bones.Api.Features.Auth.GetUserInfo.GetUserInfoResponse.Email" /> has been confirmed yet.
    /// </summary>
    public required bool IsEmailConfirmed { get; init; }
}