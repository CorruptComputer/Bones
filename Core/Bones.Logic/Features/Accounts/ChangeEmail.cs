using Bones.Database.DbSets.Accounts;
using Bones.Database.Operations.Audits;
using Bones.Logic.Features.System;
using Bones.Shared.Extensions;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;

namespace Bones.Logic.Features.Accounts;

/// <inheritdoc />
public sealed class ChangeEmail(UserManager<BonesUser> userManager, ISender sender) : IRequestHandler<ChangeEmail.Command, CommandResponse>
{
    /// <summary>
    ///   Backend request for changing a users email
    /// </summary>
    /// <param name="NewEmail"></param>
    /// <param name="UserToChange"></param>
    /// <param name="AuditUser">If the user that performed this action is not the account owner, specify who here.</param>
    public sealed record Command(string NewEmail, BonesUser UserToChange, BonesUser? AuditUser = null) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.NewEmail).NotNull().CustomAsync(async (email, ctx, cancellationToken) =>
            {
                if (!await email.IsValidEmailAsync(cancellationToken))
                {
                    ctx.AddFailure(new ValidationFailure(nameof(Command.NewEmail), "New email address is not valid."));
                }
            });
            RuleFor(x => x.UserToChange).NotNull();
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        IdentityResult emailResult = await userManager.SetEmailAsync(request.UserToChange, request.NewEmail);
        IdentityResult usernameResult = await userManager.SetUserNameAsync(request.UserToChange, request.NewEmail);

        if (emailResult.Succeeded && usernameResult.Succeeded)
        {
            await sender.Send(new QueueConfirmationEmail.Command(request.UserToChange, request.NewEmail, true), cancellationToken);
            await sender.Send(new AddAccountAuditDb.Command(
                request.UserToChange,
                Database.DbSets.Audits.AccountAudit.Actions.UpdateEmail,
                request.AuditUser ?? request.UserToChange,
                $"Changed email from {request.UserToChange.Email} to {request.NewEmail}"), cancellationToken);

            return CommandResponse.Pass();
        }

        return CommandResponse.Fail();
    }
}