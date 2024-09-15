using Bones.Database.DbSets.AccountManagement;
using Bones.Database.Operations.AccountManagement.SetEmailConfirmedDateTimeDb;
using Bones.Shared.Extensions;
using Microsoft.AspNetCore.Identity;

namespace Bones.Backend.Features.AccountManagement.ConfirmEmail;

internal class ConfirmEmailHandler(UserManager<BonesUser> userManager, ISender sender) : IRequestHandler<ConfirmEmailQuery, QueryResponse<IdentityResult>>
{
    public async Task<QueryResponse<IdentityResult>> Handle(ConfirmEmailQuery request, CancellationToken cancellationToken)
    {
        if (await userManager.FindByIdAsync(request.UserId.ToString()) is not { } user)
        {
            return IdentityResult.Failed();
        }

        string token;
        try
        {
            token = request.Code.Base64UrlSafeDecode();
        }
        catch (Exception)
        {
            return IdentityResult.Failed();
        }

        IdentityResult result;
        if (string.IsNullOrEmpty(request.ChangedEmail))
        {
            result = await userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                await sender.Send(new SetEmailConfirmedDateTimeDbCommand(user, DateTimeOffset.Now), cancellationToken);
            }
        }
        else
        {
            // Email and username are one and the same.
            result = await userManager.ChangeEmailAsync(user, request.ChangedEmail, token);

            if (result.Succeeded)
            {
                // So when we update the email, we need to update the username.
                result = await userManager.SetUserNameAsync(user, request.ChangedEmail);
            }
        }

        return result;
    }
}