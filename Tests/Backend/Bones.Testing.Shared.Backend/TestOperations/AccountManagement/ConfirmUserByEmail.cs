using Bones.Database.DbSets.AccountManagement;
using Bones.Shared.Backend.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Bones.Testing.Shared.Backend.TestOperations.AccountManagement;


public record ConfirmUserByEmailCommand(string Email) : IRequest<CommandResponse>;

public class ConfirmUserByEmail(UserManager<BonesUser> userManager) : IRequestHandler<ConfirmUserByEmailCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(ConfirmUserByEmailCommand request, CancellationToken cancellationToken)
    {
        BonesUser? user = await userManager.FindByEmailAsync(request.Email);

        if (user == null)
        {
            return CommandResponse.Fail();
        }

        if (await userManager.IsEmailConfirmedAsync(user))
        {
            return CommandResponse.Fail();
        }

        user.EmailConfirmed = true;
        user.EmailConfirmedDateTime = DateTimeOffset.Now;
        await userManager.UpdateAsync(user);

        return CommandResponse.Pass();
    }
}
