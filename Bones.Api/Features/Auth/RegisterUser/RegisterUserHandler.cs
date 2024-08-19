using System.ComponentModel.DataAnnotations;
using Bones.Api.Features.Identity.SendConfirmationEmail;
using Bones.Database.DbSets.Identity;
using Microsoft.AspNetCore.Identity;

namespace Bones.Api.Features.Identity.RegisterUser;

public class RegisterUserHandler(UserManager<BonesUser> userManager, IUserStore<BonesUser> userStore, ISender sender) : IRequestHandler<RegisterUserQuery, QueryResponse<IdentityResult>>
{
    // Validate the email address using DataAnnotations like the UserValidator does when RequireUniqueEmail = true.
    private static readonly EmailAddressAttribute _emailAddressAttribute = new();
    
    public async Task<QueryResponse<IdentityResult>> Handle(RegisterUserQuery request, CancellationToken cancellationToken)
    {
        if (!userManager.SupportsUserEmail)
        {
            throw new NotSupportedException($"{nameof(RegisterUserHandler)} requires a user store with email support.");
        }

        IUserEmailStore<BonesUser> emailStore = (IUserEmailStore<BonesUser>)userStore;
        string email = request.Email;

        if (string.IsNullOrEmpty(email) || !_emailAddressAttribute.IsValid(email))
        {
            return new()
            {
                Success = false,
                FailureReason = "Invalid email address.",
                Result = IdentityResult.Failed(userManager.ErrorDescriber.InvalidEmail(email))
            };
        }

        BonesUser user = new();
        await userStore.SetUserNameAsync(user, email, cancellationToken);
        await emailStore.SetEmailAsync(user, email, cancellationToken);
        IdentityResult result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            return new()
            {
                Success = false,
                FailureReason = result.Errors.First().Description,
                Result = result
            };
        }

        await sender.Send(new SendConfirmationEmailCommand()
        {
            Email = request.Email,
            User = user,
            Context = request.Context
        }, cancellationToken);
        
        return new()
        {
            Success = true,
            Result = result
        };
    }
}