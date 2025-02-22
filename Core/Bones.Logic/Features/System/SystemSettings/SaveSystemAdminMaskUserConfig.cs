using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.Audit;
using Bones.Database.Operations.Audit;
using Bones.Database.Operations.System.SystemSettings;
using Bones.Shared.Consts;
using Bones.Shared.Extensions;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;

namespace Bones.Logic.Features.System.SystemSettings;

/// <inheritdoc />
public class SaveSystemAdminMaskUserConfig(UserManager<BonesUser> userManager, ISender sender) : IRequestHandler<SaveSystemAdminMaskUserConfig.Command, CommandResponse>
{
    /// <summary>
    ///   Save the system admin mask user configuration
    /// </summary>
    /// <param name="IsEnabled"></param>
    /// <param name="Email">Their email address</param>
    /// <param name="DisplayName"></param>
    /// <param name="Reason"></param>
    /// <param name="ActionTakenBy"></param>
    public sealed record Command(bool IsEnabled, string Email, string DisplayName, string Reason, BonesUser ActionTakenBy) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(request => request.Email).NotNull().NotEmpty().EmailAddress().CustomAsync(async (email, ctx, cancel) =>
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
        await sender.Send(new SaveSystemAdminMaskUserEnabledDb.Command(request.IsEnabled, request.Reason, request.ActionTakenBy), cancellationToken);
        BonesUser? user = await sender.Send(new GetSystemAdminMaskUser.Query(), cancellationToken);

        if (user is null)
        {
            user = new()
            {
                UserName = request.Email,
                Email = request.Email,
                DisplayName = request.DisplayName,
                EmailConfirmed = true,
                EmailConfirmedDateTime = DateTimeOffset.Now,
                PasswordExpired = true
            };

            await userManager.CreateAsync(user);
            await userManager.AddToRoleAsync(user, SystemRoles.SYSTEM_ADMINISTRATORS);

            await sender.Send(new AddAccountAuditDb.Command(user, AccountAudit.Actions.Create, request.ActionTakenBy, request.Reason), cancellationToken);
        }
        else
        {
            user.Email = request.Email;
            user.UserName = request.Email;
            user.DisplayName = request.DisplayName;
            await userManager.UpdateAsync(user);
            await sender.Send(new AddAccountAuditDb.Command(user, AccountAudit.Actions.UpdateProfile, request.ActionTakenBy, request.Reason), cancellationToken);
        }

        return CommandResponse.Pass();
    }
}
