using Bones.Database.DbSets.AccountManagement;
using Bones.Shared.Backend.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Bones.Testing.UnitTests.Shared.TestOperations.AccountManagement;

/// <inheritdoc />
public class ConfirmUserByEmail(UserManager<BonesUser> userManager) : IRequestHandler<ConfirmUserByEmail.Command, CommandResponse>
{
    /// <summary>
    ///   TESTING COMMAND: Confirm a user by email
    /// </summary>
    /// <param name="Email"></param>
    public record Command(string Email) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
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
