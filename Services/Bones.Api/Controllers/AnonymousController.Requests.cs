using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Bones.Api.Controllers;

public sealed partial class AnonymousController
{
    /// <summary>
    ///   Request to register a new user
    /// </summary>
    /// <param name="Email">Email, must be valid and unique</param>
    /// <param name="Password">Password, must pass validation (1 upper, 1 lower, 1 number, 1 special character, and at least 8 characters long)</param>
    [Serializable]
    [JsonSerializable(typeof(RegisterUserApiRequest))]
    public sealed record RegisterUserApiRequest([Required] string Email, [Required] string Password);
}