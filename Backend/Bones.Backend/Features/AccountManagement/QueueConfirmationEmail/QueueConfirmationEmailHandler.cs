using Bones.Backend.Models;
using Bones.Database.DbSets.AccountManagement;
using Bones.Database.Operations.SystemQueues.AddConfirmationEmailToQueue;
using Bones.Shared.Consts;
using Bones.Shared.Exceptions;
using Bones.Shared.Extensions;
using Microsoft.AspNetCore.Identity;

namespace Bones.Backend.Features.AccountManagement.QueueConfirmationEmail;

internal class QueueConfirmationEmailHandler(UserManager<BonesUser> userManager, BackendConfiguration config, ISender sender) : IRequestHandler<QueueConfirmationEmailCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(QueueConfirmationEmailCommand request, CancellationToken cancellationToken)
    {
        if (config.WebUIBaseUrl is null)
        {
            throw new BonesException("BackendConfiguration:WebUIBaseUrl is null in appsettings");
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

        return await sender.Send(new AddConfirmationEmailToQueueDbCommand(request.Email, builder.ToString()), cancellationToken);
    }
}