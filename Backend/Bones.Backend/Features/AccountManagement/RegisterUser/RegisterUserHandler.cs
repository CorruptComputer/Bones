using Bones.Backend.Features.AccountManagement.QueueConfirmationEmail;
using Bones.Database.DbSets.AccountManagement;
using Bones.Shared.Exceptions;
using Bones.Shared.Extensions;
using Microsoft.AspNetCore.Identity;

namespace Bones.Backend.Features.AccountManagement.RegisterUser;

internal class RegisterUserHandler(UserManager<BonesUser> userManager, ISender sender) : IRequestHandler<RegisterUserQuery, QueryResponse<IdentityResult>>
{
    public async Task<QueryResponse<IdentityResult>> Handle(RegisterUserQuery request, CancellationToken cancellationToken)
    {
        if (!userManager.SupportsUserEmail)
        {
            throw new BonesException($"{nameof(RegisterUserHandler)} requires a user store with email support.");
        }

        if (string.IsNullOrEmpty(request.Email) || !await request.Email.IsValidEmailAsync(cancellationToken))
        {
            return IdentityResult.Failed(userManager.ErrorDescriber.InvalidEmail(request.Email));
        }

        BonesUser user = new();

        await userManager.SetUserNameAsync(user, request.Email);
        await userManager.SetEmailAsync(user, request.Email);

        IdentityResult result = await userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
        {
            await sender.Send(new QueueConfirmationEmailCommand(user, request.Email), cancellationToken);
        }

        return result;
    }
}