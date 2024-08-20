using System.Text;
using Bones.Database.DbSets.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

namespace Bones.Api.Features.Auth.ConfirmEmail;

public class ConfirmEmailHandler(UserManager<BonesUser> userManager) : IRequestHandler<ConfirmEmailQuery, QueryResponse<IdentityResult>>
{
    public async Task<QueryResponse<IdentityResult>> Handle(ConfirmEmailQuery request, CancellationToken cancellationToken)
    {
        if (await userManager.FindByIdAsync(request.UserId) is not { } user)
        {
            return new()
            {
                Success = false,
                FailureReason = "User does not exist"
            };
        }

        string decodedCode;
        try
        {
            decodedCode = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));
        }
        catch (FormatException)
        {
            return new()
            {
                Success = false,
                FailureReason = "Failed to decode email code"
            };
        }

        IdentityResult result;

        if (string.IsNullOrEmpty(request.ChangedEmail))
        {
            result = await userManager.ConfirmEmailAsync(user, decodedCode);
        }
        else
        {
            // As with Identity UI, email and user name are one and the same. So when we update the email,
            // we need to update the user name.
            result = await userManager.ChangeEmailAsync(user, request.ChangedEmail, decodedCode);

            if (result.Succeeded)
            {
                result = await userManager.SetUserNameAsync(user, request.ChangedEmail);
            }
        }

        if (!result.Succeeded)
        {
            return new()
            {
                Success = false,
                FailureReason = "The code is invalid"
            };
        }

        return result;
    }
}