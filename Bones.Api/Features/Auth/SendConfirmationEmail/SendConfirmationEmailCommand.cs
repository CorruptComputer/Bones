using Bones.Database.DbSets.Identity;

namespace Bones.Api.Features.Auth.SendConfirmationEmail;

public class SendConfirmationEmailCommand : IValidatableRequest<CommandResponse>
{
    
    public required BonesUser User { get; init; }
    public required HttpContext Context { get; init; }
    public required string Email { get; init; }
    public bool IsChange { get; init; } = false;
    
    public (bool valid, string? invalidReason) IsRequestValid()
    {
        throw new NotImplementedException();
    }
}