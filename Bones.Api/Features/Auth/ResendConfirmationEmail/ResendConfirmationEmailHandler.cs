using Bones.Api.Features.Auth.SendConfirmationEmail;
using Bones.Database.DbSets.Identity;
using Bones.Shared.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace Bones.Api.Features.Auth.ResendConfirmationEmail;

public class ResendConfirmationEmailHandler(UserManager<BonesUser> userManager,  ISender sender) : IRequestHandler<ResendConfirmationEmailCommand, CommandResponse>
{
   
    public async Task<CommandResponse> Handle(ResendConfirmationEmailCommand request, CancellationToken cancellationToken)
    {
        if (await userManager.FindByEmailAsync(request.Email) is not { } user)
        {
            return new()
            {
                Success = false,
                FailureReason = "User not found"
            };
        }
        
        return await sender.Send(new SendConfirmationEmailCommand()
        {
            Email = request.Email,
            User = user,
            Context = request.Context ?? throw new BonesException("Context is null")
        }, cancellationToken);
    }
}