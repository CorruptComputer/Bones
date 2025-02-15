using Bones.Database.DbSets.AccountManagement;
using Bones.Database.Operations.AccountManagement;
using Bones.Shared.Extensions;
using Microsoft.AspNetCore.Identity;

namespace Bones.Logic.Features.Accounts;

/// <summary>
///   Backend request for confirming a users email
/// </summary>
/// <param name="UserId"></param>
/// <param name="Code"></param>
/// <param name="ChangedEmail"></param>
public sealed record ConfirmEmailQuery(Guid UserId, string Code, string? ChangedEmail) : IRequest<QueryResponse<IdentityResult>>;

internal class ConfirmEmailQueryValidator : AbstractValidator<ConfirmEmailQuery>
{
    public ConfirmEmailQueryValidator()
    {
        RuleFor(x => x.UserId).NotNull().NotEmpty();
        RuleFor(x => x.Code).NotNull().NotEmpty();
        RuleFor(x => x.ChangedEmail).Custom(async (email, ctx) =>
        {
            if (email != null && !await email.IsValidEmailAsync())
            {
                ctx.AddFailure("ChangedEmail", "Email is not valid");
            }
        });
    }
}

internal class ConfirmEmailHandler(UserManager<BonesUser> userManager, ISender sender) : IRequestHandler<ConfirmEmailQuery, QueryResponse<IdentityResult>>
{
    public async Task<QueryResponse<IdentityResult>> Handle(ConfirmEmailQuery request, CancellationToken cancellationToken)
    {
        if (await userManager.FindByIdAsync(request.UserId.ToString()) is not { } user)
        {
            return IdentityResult.Failed();
        }

        string token;
        try
        {
            token = request.Code.Base64UrlSafeDecode();
        }
        catch (Exception)
        {
            return IdentityResult.Failed();
        }

        IdentityResult result;
        if (string.IsNullOrEmpty(request.ChangedEmail))
        {
            result = await userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                await sender.Send(new SetEmailConfirmedDateTimeDb.Command(user.Id, DateTimeOffset.Now), cancellationToken);
            }
        }
        else
        {
            // Email and username are one and the same.
            result = await userManager.ChangeEmailAsync(user, request.ChangedEmail, token);

            if (result.Succeeded)
            {
                // When we update the email, we need to update the username to match.
                result = await userManager.SetUserNameAsync(user, request.ChangedEmail);
            }
        }

        return result;
    }
}