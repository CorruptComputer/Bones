using Bones.Backend.Features.AccountManagement.QueueConfirmationEmail;
using Bones.Database.DbSets.AccountManagement;
using Bones.Shared.Backend.Models;
using Microsoft.AspNetCore.Identity;

namespace Bones.Backend.Features.AccountManagement.ResendConfirmationEmail;

public class ResendConfirmationEmailHandler(UserManager<BonesUser> userManager, ISender sender) : IRequestHandler<ResendConfirmationEmailRequest, CommandResponse>
{
    public async Task<CommandResponse> Handle(ResendConfirmationEmailRequest request, CancellationToken cancellationToken)
    {
        if (await userManager.FindByEmailAsync(request.Email) is not { } user)
        {
            return CommandResponse.Fail();
        }

        await sender.Send(new QueueConfirmationEmailRequest(user, request.Email), cancellationToken);

        return CommandResponse.Pass(user.Id);
    }
}