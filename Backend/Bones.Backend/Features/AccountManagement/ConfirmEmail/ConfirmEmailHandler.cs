using Bones.Database.DbSets.AccountManagement;
using Bones.Shared.Backend.Models;
using Bones.Shared.Extensions;
using Microsoft.AspNetCore.Identity;

namespace Bones.Backend.Features.AccountManagement.ConfirmEmail;

public class ConfirmEmailHandler(UserManager<BonesUser> userManager) : IRequestHandler<ConfirmEmailRequest, QueryResponse<IdentityResult>>
{
    public async Task<QueryResponse<IdentityResult>> Handle(ConfirmEmailRequest request, CancellationToken cancellationToken)
    {
        if (await userManager.FindByIdAsync(request.UserId.ToString()) is not { } user)
        {
            return IdentityResult.Failed();
        }

        string decodedCode;
        try
        {
            decodedCode = request.Code.Base64UrlSafeDecode();
        }
        catch (Exception)
        {
            return IdentityResult.Failed();
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

        return result;
    }
}