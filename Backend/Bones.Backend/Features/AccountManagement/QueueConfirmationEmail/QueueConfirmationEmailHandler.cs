using System.Text.Encodings.Web;
using Bones.Backend.Models;
using Bones.Database.DbSets.AccountManagement;
using Bones.Shared.Backend.Models;
using Bones.Shared.Consts;
using Bones.Shared.Extensions;
using Microsoft.AspNetCore.Identity;

namespace Bones.Backend.Features.AccountManagement.QueueConfirmationEmail;

public class QueueConfirmationEmailHandler(UserManager<BonesUser> userManager, BackendConfiguration config) : IRequestHandler<QueueConfirmationEmailRequest, CommandResponse>
{
    public async Task<CommandResponse> Handle(QueueConfirmationEmailRequest request, CancellationToken cancellationToken)
    {
        if (config.WebUIBaseUrl is null)
        {
            throw new ApplicationException("BackendConfiguration:WebUIBaseUrl is null in appsettings");
        }

        string code = request.IsChange
            ? await userManager.GenerateChangeEmailTokenAsync(request.User, request.Email)
            : await userManager.GenerateEmailConfirmationTokenAsync(request.User);

        code = code.Base64UrlSafeEncode();

        UriBuilder builder = new(config.WebUIBaseUrl)
        {
            Path = FrontEndUrls.Account.CONFIRM_EMAIL
        };

        string userId = await userManager.GetUserIdAsync(request.User);

        builder.Query = $"?userId={userId}&code={code}";

        if (request.IsChange)
        {
            builder.Query += $"&changedEmail={request.Email}";
        }

        // TODO: Add to DB here

        return new()
        {
            Success = true
        };
    }
}