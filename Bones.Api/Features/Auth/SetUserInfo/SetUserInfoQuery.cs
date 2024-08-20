using System.Text.Json.Serialization;
using Bones.Api.Features.Auth.GetUserInfo;

namespace Bones.Api.Features.Auth.SetUserInfo;

[JsonSerializable(typeof(SetUserInfoQuery))]
public sealed record SetUserInfoQuery : IValidatableRequest<QueryResponse<GetUserInfoResponse>>
{
    
    /// <summary>
    /// The optional new email address for the authenticated user. This will replace the old email address if there was one. The email will not be updated until it is confirmed.
    /// </summary>
    public string? NewEmail { get; init; }

    /// <summary>
    /// The optional new password for the authenticated user. If a new password is provided, the <see cref="P:Bones.Api.Features.Auth.SetUserInfo.SetUserInfoQuery.OldPassword" /> is required.
    /// If the user forgot the old password, use the "/forgotPassword" endpoint instead.
    /// </summary>
    public string? NewPassword { get; init; }

    /// <summary>
    /// The old password for the authenticated user. This is only required if a <see cref="P:Bones.Api.Features.Auth.SetUserInfo.SetUserInfoQuery.NewPassword" /> is provided.
    /// </summary>
    public string? OldPassword { get; init; }
    
    public (bool valid, string? invalidReason) IsRequestValid()
    {
        throw new NotImplementedException();
    }
}