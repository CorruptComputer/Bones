using Bones.Backend.Features.AccountManagement.QueueConfirmationEmail;
using Bones.Database.DbSets.AccountManagement;
using Bones.Shared.Exceptions;
using Bones.Shared.Extensions;
using Microsoft.AspNetCore.Identity;

namespace Bones.Backend.Features.AccountManagement.RegisterUser;

internal class RegisterUserHandler(UserManager<BonesUser> userManager, IUserEmailStore<BonesUser> userStore, ISender sender) : IRequestHandler<RegisterUserRequest, QueryResponse<IdentityResult>>
{
    public async Task<QueryResponse<IdentityResult>> Handle(RegisterUserRequest request, CancellationToken cancellationToken)
    {
        if (!userManager.SupportsUserEmail)
        {
            throw new BonesException($"{nameof(RegisterUserHandler)} requires a user store with email support.");
        }

        if (string.IsNullOrEmpty(request.Email) || !await request.Email.IsValidEmailAsync())
        {
            return IdentityResult.Failed(userManager.ErrorDescriber.InvalidEmail(request.Email));
        }

        BonesUser user = new();

        await userStore.SetUserNameAsync(user, request.Email, cancellationToken);
        await userStore.SetEmailAsync(user, request.Email, cancellationToken);

        IdentityResult result = await userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
        {
            await sender.Send(new QueueConfirmationEmailRequest(user, request.Email), cancellationToken);
        }

        return result;
    }
}