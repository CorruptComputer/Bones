namespace Bones.Api.Models.Login;

/// <summary>
///   Request to login
/// </summary>
[JsonSerializable(typeof(LoginUserApiRequest))]
public sealed record LoginUserApiRequest
{
    /// <summary>
    ///   The users email address
    /// </summary>
    [JsonRequired]
    public required string Email { get; init; }

    /// <summary>
    ///   The users password
    /// </summary>
    [JsonRequired]
    public required string Password { get; init; }

    /// <summary>
    ///   If they have 2fa, include the code here
    /// </summary>
    public string? TwoFactorCode { get; init; }

    /// <summary>
    ///   If they have 2fa and can't use their authenticator, include a recovery code here
    /// </summary>
    public string? TwoFactorRecoveryCode { get; init; }
}