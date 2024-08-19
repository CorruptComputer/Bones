using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace Bones.Api.Features.Identity.RegisterUser;

[JsonSerializable(typeof(RegisterUserQuery))]
public sealed record RegisterUserQuery : IValidatableRequest<QueryResponse<IdentityResult>>
{
    /// <summary>
    ///   The user's email.
    /// </summary>
    [Required]
    public required string Email { get; init; }

    /// <summary>
    ///   The user's password.
    /// </summary>
    [Required]
    public required string Password { get; init; }
    
    [JsonIgnore]
    public required HttpContext Context { get; set; }
    
    public (bool valid, string? invalidReason) IsRequestValid()
    {
        throw new NotImplementedException();
    }
}