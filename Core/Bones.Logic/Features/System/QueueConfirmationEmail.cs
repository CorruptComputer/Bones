using Bones.Database.DbSets.Accounts;
using Bones.Database.Operations.System.Queues;
using Bones.Database.Operations.System.SystemSettings;
using Bones.Shared.Exceptions;
using Bones.Shared.Extensions;
using Microsoft.AspNetCore.Identity;

namespace Bones.Logic.Features.System;

/// <inheritdoc />
public class QueueConfirmationEmail(UserManager<BonesUser> userManager, ISender sender) : IRequestHandler<QueueConfirmationEmail.Command, CommandResponse>
{
    /// <summary>
    ///   Backend command for queueing a confirmation email.
    /// </summary>
    /// <param name="User"></param>
    /// <param name="Email"></param>
    /// <param name="IsChange"></param>
    public sealed record Command(BonesUser User, string Email, bool IsChange = false) : IRequest<CommandResponse>;

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
                    ctx.AddFailure(nameof(Command.Email), "Email is invalid");
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

        string code = request.IsChange
            ? await userManager.GenerateChangeEmailTokenAsync(request.User, request.Email)
            : await userManager.GenerateEmailConfirmationTokenAsync(request.User);

        code = code.Base64UrlSafeEncode();

        UriBuilder builder = new(webUiBaseUrl);

        string userId = await userManager.GetUserIdAsync(request.User);

        builder.Query = $"?userId={userId}&code={code}";

        if (request.IsChange)
        {
            builder.Query += $"&changedEmail={request.Email}";
        }

        return await sender.Send(new AddConfirmationEmailToQueueDb.Command(request.Email, builder.ToString()), cancellationToken);
    }
}