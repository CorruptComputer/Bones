using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Bones.Api.Controllers;

public sealed partial class LoginController
{
    /// <summary>
    ///   Request to login
    /// </summary>
    /// <param name="Email">The users email address</param>
    /// <param name="Password">The users password</param>
    /// <param name="TwoFactorCode">If they have 2fa, include the code here</param>
    /// <param name="TwoFactorRecoveryCode">If they have 2fa and can't use their authenticator, include a recovery code here</param>
    [Serializable]
    [JsonSerializable(typeof(LoginUserApiRequest))]
    public sealed record LoginUserApiRequest([Required] string Email, [Required] string Password, string? TwoFactorCode, string? TwoFactorRecoveryCode);
}