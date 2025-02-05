using Bones.Database.DbSets.AccountManagement;
using Bones.Shared.Extensions;
using Microsoft.AspNetCore.Identity;

namespace Bones.Logic.Features.Accounts;

/// <summary>
///   Backend request for resending the confirmation email.
/// </summary>
/// <param name="Email">The email address to resent the confirmation request to.</param>
public sealed record QueueResendConfirmationEmailCommand(string Email) : IRequest<CommandResponse>;

internal class QueueResendConfirmationEmailCommandValidator : AbstractValidator<QueueResendConfirmationEmailCommand>
{
    public QueueResendConfirmationEmailCommandValidator()
    {
        RuleFor(x => x.Email).NotNull().NotEmpty().EmailAddress().CustomAsync(async (email, ctx, cancel) =>
        {
            if (!await email.IsValidEmailAsync(cancel))
            {
                ctx.AddFailure(nameof(QueueResendConfirmationEmailCommand.Email), "Email domain is invalid");
            }
        });
    }
}

internal class QueueResendConfirmationEmailHandler(UserManager<BonesUser> userManager, ISender sender) : IRequestHandler<QueueResendConfirmationEmailCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(QueueResendConfirmationEmailCommand request, CancellationToken cancellationToken)
    {
        if (await userManager.FindByEmailAsync(request.Email) is not { } user)
        {
            return CommandResponse.Fail();
        }

        await sender.Send(new QueueConfirmationEmailCommand(user, request.Email), cancellationToken);

        return CommandResponse.Pass(user.Id);
    }
}