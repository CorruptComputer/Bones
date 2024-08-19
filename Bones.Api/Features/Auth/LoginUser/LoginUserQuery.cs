using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace Bones.Api.Features.Identity.LoginUser;

[JsonSerializable(typeof(LoginUserQuery))]
public sealed record LoginUserQuery : IValidatableRequest<QueryResponse<SignInResult>>
{
    [Required]
    public required string Email { get; init; }

    /// <summary>The user's password.</summary>
    [Required]
    public required string Password { get; init; }

    /// <summary>
    /// The optional two-factor authenticator code. This may be required for users who have enabled two-factor authentication.
    /// This is not required if a <see cref="P:Microsoft.AspNetCore.Identity.Data.LoginRequest.TwoFactorRecoveryCode" /> is sent.
    /// </summary>
    public string? TwoFactorCode { get; init; }

    /// <summary>
    /// An optional two-factor recovery code from <see cref="P:Microsoft.AspNetCore.Identity.Data.TwoFactorResponse.RecoveryCodes" />.
    /// This is required for users who have enabled two-factor authentication but lost access to their <see cref="P:Microsoft.AspNetCore.Identity.Data.LoginRequest.TwoFactorCode" />.
    /// </summary>
    public string? TwoFactorRecoveryCode { get; init; }

    [JsonIgnore]
    public bool? UseCookies { get; set; }
    
    [JsonIgnore]
    public bool? UseSessionCookies { get; set; }
    
    public (bool valid, string? invalidReason) IsRequestValid()
    {
        throw new NotImplementedException();
    }
}