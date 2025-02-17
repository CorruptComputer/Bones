using System.ComponentModel.DataAnnotations;
using Bones.Database.DbSets.AccountManagement;
using Bones.Database.Operations.System;
using Bones.Logic.Models;
using Bones.Shared.Consts;
using Bones.Shared.Exceptions;
using Bones.Shared.Extensions;
using Microsoft.AspNetCore.Identity;

namespace Bones.Logic.Features.System;

/// <inheritdoc />
public class QueueForgotPasswordEmail(UserManager<BonesUser> userManager, BackendConfiguration config, ISender sender) : IRequestHandler<QueueForgotPasswordEmail.Command, CommandResponse>
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
            RuleFor(x => x.Email).NotNull().NotEmpty().EmailAddress().CustomAsync(async (email, ctx, cancel) =>
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

            await sender.Send(new AddForgotPasswordEmailToQueueDb.Command(request.Email, builder.ToString()), cancellationToken);
        }

        // Don't reveal that the user does not exist or is not confirmed
        return CommandResponse.Pass();
    }
}