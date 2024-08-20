using System.Text.Json.Serialization;

namespace Bones.Api.Features.Auth.ResendConfirmationEmail;

public class ResendConfirmationEmailCommand : IValidatableRequest<CommandResponse>
{
    public required string Email { get; init; }
    
    [JsonIgnore]
    public HttpContext? Context { get; set; }
    
    public (bool valid, string? invalidReason) IsRequestValid()
    {
        throw new NotImplementedException();
    }
}