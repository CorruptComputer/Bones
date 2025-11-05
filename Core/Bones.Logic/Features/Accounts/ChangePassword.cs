using Bones.Database.DbSets.AccountManagement;
using Bones.Database.Operations.AccountManagement;
using Bones.Database.Operations.Audit;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;

namespace Bones.Logic.Features.Accounts;

/// <inheritdoc />
public sealed class ChangePassword(UserManager<BonesUser> userManager, ISender sender) : IRequestHandler<ChangePassword.Command, CommandResponse>
{
    /// <summary>
    ///   Backend request for changing a users email
    /// </summary>
    /// <param name="NewPassword"></param>
    /// <param name="CurrentPassword"></param>
    /// <param name="InvalidateOtherSessions"></param>
    /// <param name="CurrentSessionId"></param>
    /// <param name="UserToChange">This should only ever be done by the current logged in user</param>
    public sealed record Command(string CurrentPassword, string NewPassword, bool InvalidateOtherSessions, Guid? CurrentSessionId, BonesUser UserToChange) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.CurrentPassword).NotEmpty();

            RuleFor(x => x.NewPassword).NotNull().MinimumLength(8).Custom((password, ctx) =>
            {
                if (string.IsNullOrWhiteSpace(password))
                {
                    return;
                }

                if (!password.Any(char.IsUpper))
                {
                    ctx.AddFailure(new ValidationFailure(nameof(Command.NewPassword), "NewPassword must contain at least one capital letter"));
                }

                if (!password.Any(char.IsLower))
                {
                    ctx.AddFailure(new ValidationFailure(nameof(Command.NewPassword), "NewPassword must contain at least one lowercase letter"));
                }

                if (!password.Any(char.IsDigit))
                {
                    ctx.AddFailure(new ValidationFailure(nameof(Command.NewPassword), "NewPassword must contain at least one digit"));
                }

                if (!password.Any(c => !char.IsUpper(c) && !char.IsLower(c) && !char.IsDigit(c)))
                {
                    ctx.AddFailure(new ValidationFailure(nameof(Command.NewPassword), "NewPassword must contain at least one special character"));
                }
            });

            RuleFor(x => x.InvalidateOtherSessions).NotNull();
            RuleFor(x => x.CurrentSessionId).NotNull().When(x => x.InvalidateOtherSessions == true);
            RuleFor(x => x.UserToChange).NotNull();
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        IdentityResult result = await userManager.ChangePasswordAsync(request.UserToChange, request.CurrentPassword, request.NewPassword);

        if (result.Succeeded)
        {
            await sender.Send(new AddAccountAuditDb.Command(
                request.UserToChange,
                Database.DbSets.Audit.AccountAudit.Actions.UpdatePassword,
                request.UserToChange,
                $"Password changed{(request.InvalidateOtherSessions == true ? ", other sessions invalidated." : string.Empty)}"), cancellationToken);

            if (request.InvalidateOtherSessions && request.CurrentSessionId is not null)
            {
                await sender.Send(new InvalidateAllSessionsForUserDb.Command(request.UserToChange.Id, [request.CurrentSessionId.Value]), cancellationToken);
            }

            return CommandResponse.Pass();
        }

        return CommandResponse.Fail();
    }
}