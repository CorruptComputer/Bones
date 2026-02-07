using Bones.Database.DbSets.Accounts;
using Bones.Database.DbSets.Audits;
using Bones.Database.Operations.Audits;
using Bones.Shared.Extensions;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;

namespace Bones.Logic.Features.System.SystemSettings;

/// <inheritdoc />
public class SaveBackgroundServiceUserConfig(UserManager<BonesUser> userManager, ISender sender) : IRequestHandler<SaveBackgroundServiceUserConfig.Command, CommandResponse>
{
    /// <summary>
    ///   Command to save the background service user configuration
    /// </summary>
    /// <param name="Email">Their email address</param>
    /// <param name="DisplayName"></param>
    /// <param name="Reason"></param>
    /// <param name="ActionTakenBy"></param>
    public sealed record Command(string Email, string DisplayName, string Reason, BonesUser ActionTakenBy) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(request => request.Email).NotEmpty().EmailAddress().CustomAsync(async (email, ctx, cancel) =>
            {
                if (!await email.IsValidEmailAsync(cancel))
                {
                    ctx.AddFailure(new ValidationFailure(nameof(Command.Email), "Email domain is invalid"));
                }
            });

            RuleFor(request => request.DisplayName).NotEmpty();
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        BonesUser? user = await sender.Send(new GetBackgroundServiceUser.Query(), cancellationToken);

        if (user is null)
        {
            return CommandResponse.Fail("User not found");
        }

        user.Email = request.Email;
        user.UserName = request.Email;
        user.DisplayName = request.DisplayName;

        await userManager.UpdateAsync(user);

        await sender.Send(new AddAccountAuditDb.Command(user, AccountAudit.Actions.UpdateProfile, request.ActionTakenBy, request.Reason), cancellationToken);

        return CommandResponse.Pass();
    }
}
