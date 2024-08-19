using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace Bones.Api.Features.Identity.ConfirmEmail;

[JsonSerializable(typeof(ConfirmEmailQuery))]
public sealed record ConfirmEmailQuery : IValidatableRequest<QueryResponse<IdentityResult>>
{
     public required string UserId { get; init; }
     public required string Code { get; init; }
     public string? ChangedEmail { get; init; }
     
     public (bool valid, string? invalidReason) IsRequestValid()
     {
          throw new NotImplementedException();
     }
}