using Bones.Backend.Features.AccountManagement.QueueConfirmationEmail;
using Bones.Database.DbSets.AccountManagement;
using Microsoft.AspNetCore.Identity;

namespace Bones.Backend.Features.AccountManagement.ResendConfirmationEmail;

internal class ResendConfirmationEmailHandler(UserManager<BonesUser> userManager, ISender sender) : IRequestHandler<ResendConfirmationEmailRequest, CommandResponse>
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