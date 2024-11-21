using Bones.Database.DbSets.AccountManagement;
using Bones.Logic.Features.Accounts.QueueConfirmationEmail;
using Microsoft.AspNetCore.Identity;

namespace Bones.Logic.Features.Accounts.QueueResendConfirmationEmail;

internal class QueueResendConfirmationEmailHandler(UserManager<BonesUser> userManager, ISender sender) : IRequestHandler<QueueResendConfirmationEmailCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(QueueResendConfirmationEmailCommand request, CancellationToken cancellationToken)
    {
        if (await userManager.FindByEmailAsync(request.Email) is not { } user)
        {
            return CommandResponse.Fail();
        }

        await sender.Send(new QueueConfirmationEmailCommand(user, request.Email), cancellationToken);

        return CommandResponse.Pass(user.Id);
    }
}