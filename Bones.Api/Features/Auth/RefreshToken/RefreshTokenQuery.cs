using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.Json.Serialization;

namespace Bones.Api.Features.Identity.RefreshToken;

[JsonSerializable(typeof(RefreshTokenQuery))]
public sealed record RefreshTokenQuery : IValidatableRequest<QueryResponse<ClaimsPrincipal>>
{
    [Required]
    public required string RefreshToken { get; init; }

    public (bool valid, string? invalidReason) IsRequestValid()
    {
        throw new NotImplementedException();
    }
}