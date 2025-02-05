using Bones.Database.DbSets.AccountManagement;
using Bones.Database.Operations.SystemQueues.ConfirmationEmail.AddConfirmationEmailToQueueDb;
using Bones.Logic.Models;
using Bones.Shared.Consts;
using Bones.Shared.Exceptions;
using Bones.Shared.Extensions;
using Microsoft.AspNetCore.Identity;

namespace Bones.Logic.Features.Accounts;

/// <summary>
///   Backend command for queueing a confirmation email.
/// </summary>
/// <param name="User"></param>
/// <param name="Email"></param>
/// <param name="IsChange"></param>
public sealed record QueueConfirmationEmailCommand(BonesUser User, string Email, bool IsChange = false) : IRequest<CommandResponse>;

internal class QueueConfirmationEmailCommandValidator : AbstractValidator<QueueConfirmationEmailCommand>
{
    public QueueConfirmationEmailCommandValidator()
    {
        RuleFor(x => x.Email).NotNull().NotEmpty().EmailAddress().CustomAsync(async (email, ctx, cancel) =>
        {
            if (!await email.IsValidEmailAsync(cancel))
            {
                ctx.AddFailure(nameof(QueueConfirmationEmailCommand.Email), "Email is invalid");
            }
        });
    }
}

internal class QueueConfirmationEmailHandler(UserManager<BonesUser> userManager, BackendConfiguration config, ISender sender) : IRequestHandler<QueueConfirmationEmailCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(QueueConfirmationEmailCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(config.WebUIBaseUrl))
        {
            throw new BonesException("BackendConfiguration:WebUIBaseUrl is not set in appsettings");
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