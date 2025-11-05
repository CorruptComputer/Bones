using System.ComponentModel.DataAnnotations;
using Bones.Database.DbSets.AccountManagement;
using Bones.Database.Operations.System.Queues;
using Bones.Database.Operations.System.SystemSettings;
using Bones.Shared.Consts;
using Bones.Shared.Exceptions;
using Bones.Shared.Extensions;
using Microsoft.AspNetCore.Identity;

namespace Bones.Logic.Features.System;

/// <inheritdoc />
public class QueueForgotPasswordEmail(UserManager<BonesUser> userManager, ISender sender) : IRequestHandler<QueueForgotPasswordEmail.Command, CommandResponse>
{
    /// <summary>
    ///   Backend request for queueing a password reset email.
    /// </summary>
    /// <param name="Email"></param>
    public sealed record Command([Required] string Email) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress().CustomAsync(async (email, ctx, cancel) =>
            {
                if (!await email.IsValidEmailAsync(cancel))
                {
                    ctx.AddFailure(nameof(Command.Email), "Email domain is invalid");
                }
            });
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        string? webUiBaseUrl = await sender.Send(new GetWebUiBaseUrlDb.Query(), cancellationToken);
        if (string.IsNullOrEmpty(webUiBaseUrl))
        {
            throw new BonesException("Web UI base URL is not set in system settings.");
        }

        BonesUser? user = await userManager.FindByEmailAsync(request.Email);
        if (user is not null && await userManager.IsEmailConfirmedAsync(user))
        {
            string code = await userManager.GeneratePasswordResetTokenAsync(user);
            code = code.Base64UrlSafeEncode();

            // Generate ResetPassword URL
            UriBuilder builder = new(webUiBaseUrl)
            {
                Path = FrontEndUrls.Account.RESET_PASSWORD,
                Query = $"?email={request.Email}&code={code}"
            };

            await sender.Send(new AddForgotPasswordEmailToQueueDb.Command(request.Email, builder.ToString()), cancellationToken);
        }

        // Don't reveal that the user does not exist or is not confirmed
        return CommandResponse.Pass();
    }
}