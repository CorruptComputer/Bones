using System.ComponentModel.DataAnnotations;
using Bones.Database.DbSets.AccountManagement;
using Bones.Database.Operations.SystemQueues.ForgotPassword.AddForgotPasswordEmailToQueueDb;
using Bones.Logic.Models;
using Bones.Shared.Consts;
using Bones.Shared.Exceptions;
using Bones.Shared.Extensions;
using Microsoft.AspNetCore.Identity;

namespace Bones.Logic.Features.Accounts;

/// <summary>
///   Backend request for queueing a password reset email.
/// </summary>
/// <param name="Email"></param>
public sealed record QueueForgotPasswordEmailCommand([Required] string Email) : IRequest<CommandResponse>;

internal class QueueForgotPasswordEmailCommandValidator : AbstractValidator<QueueForgotPasswordEmailCommand>
{
    public QueueForgotPasswordEmailCommandValidator()
    {
        RuleFor(x => x.Email).NotNull().NotEmpty().EmailAddress().CustomAsync(async (email, ctx, cancel) =>
        {
            if (!await email.IsValidEmailAsync(cancel))
            {
                ctx.AddFailure(nameof(QueueForgotPasswordEmailCommand.Email), "Email domain is invalid");
            }
        });
    }
}

internal class QueueForgotPasswordEmailHandler(UserManager<BonesUser> userManager, BackendConfiguration config, ISender sender) : IRequestHandler<QueueForgotPasswordEmailCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(QueueForgotPasswordEmailCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(config.WebUIBaseUrl))
        {
            throw new BonesException("BackendConfiguration:WebUIBaseUrl is not set in appsettings");
        }

        BonesUser? user = await userManager.FindByEmailAsync(request.Email);
        if (user is not null && await userManager.IsEmailConfirmedAsync(user))
        {
            string code = await userManager.GeneratePasswordResetTokenAsync(user);
            code = code.Base64UrlSafeEncode();

            // Generate ResetPassword URL
            UriBuilder builder = new(config.WebUIBaseUrl)
            {
                Path = FrontEndUrls.Account.RESET_PASSWORD,
                Query = $"?email={request.Email}&code={code}"
            };

            await sender.Send(new AddForgotPasswordEmailToQueueDbCommand(request.Email, builder.ToString()), cancellationToken);
        }

        // Don't reveal that the user does not exist or is not confirmed
        return CommandResponse.Pass();
    }
}