using Bones.Database.DbSets.AccountManagement;
using Bones.Shared.Extensions;
using Microsoft.AspNetCore.Identity;

namespace Bones.Logic.Features.System;

/// <inheritdoc />
public class QueueResendConfirmationEmail(UserManager<BonesUser> userManager, ISender sender) : IRequestHandler<QueueResendConfirmationEmail.Command, CommandResponse>
{
    /// <summary>
    ///   Backend request for resending the confirmation email.
    /// </summary>
    /// <param name="Email">The email address to resent the confirmation request to.</param>
    public sealed record Command(string Email) : IRequest<CommandResponse>;

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
        if (await userManager.FindByEmailAsync(request.Email) is not { } user)
        {
            return CommandResponse.Fail();
        }

        await sender.Send(new QueueConfirmationEmail.Command(user, request.Email), cancellationToken);

        return CommandResponse.Pass();
    }
}