namespace Bones.Api.Models.Anonymous;

/// <summary>
///   Request to register a new user
/// </summary>
[JsonSerializable(typeof(RegisterUserApiRequest))]
public sealed record RegisterUserApiRequest
{
    /// <summary>
    ///   Email, must be valid and unique
    /// </summary>
    [JsonRequired]
    public required string Email { get; init; }

    /// <summary>
    ///   Password, must pass validation (1 upper, 1 lower, 1 number, 1 special character, and at least 8 characters long)
    /// </summary>
    [JsonRequired]
    public required string Password { get; init; }
}