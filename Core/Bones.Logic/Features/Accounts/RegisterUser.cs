using Bones.Database.DbSets.Accounts;
using Bones.Database.Operations.Accounts;
using Bones.Database.Operations.Audits;
using Bones.Logic.Features.System;
using Bones.Shared.Exceptions;
using Bones.Shared.Extensions;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;

namespace Bones.Logic.Features.Accounts;

/// <inheritdoc />
public class RegisterUser(UserManager<BonesUser> userManager, ISender sender) : IRequestHandler<RegisterUser.Query, QueryResponse<IdentityResult>>
{
    /// <summary>
    ///   Backend request for registering a new user.
    /// </summary>
    /// <param name="Email">Their email address</param>
    /// <param name="Password">Their desired password</param>
    public sealed record Query(string Email, string Password) : IRequest<QueryResponse<IdentityResult>>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Query>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(request => request.Email).NotEmpty().EmailAddress().CustomAsync(async (email, ctx, cancel) =>
            {
                if (!await email.IsValidEmailAsync(cancel))
                {
                    ctx.AddFailure(new ValidationFailure(nameof(Query.Email), "Email domain is invalid"));
                }
            });

            RuleFor(request => request.Password).NotEmpty().MinimumLength(8).Custom((password, ctx) =>
            {
                if (string.IsNullOrWhiteSpace(password))
                {
                    // Already handled by NotEmpty above, so the failure is not needed here.
                    // char.IsUpper below will throw if its null though.
                    return;
                }

                if (!password.Any(char.IsUpper))
                {
                    ctx.AddFailure(new ValidationFailure(nameof(Query.Password), "Password must contain at least one capital letter"));
                }

                if (!password.Any(char.IsLower))
                {
                    ctx.AddFailure(new ValidationFailure(nameof(Query.Password), "Password must contain at least one lowercase letter"));
                }

                if (!password.Any(char.IsDigit))
                {
                    ctx.AddFailure(new ValidationFailure(nameof(Query.Password), "Password must contain at least one digit"));
                }

                if (!password.Any(c => !char.IsUpper(c) && !char.IsLower(c) && !char.IsDigit(c)))
                {
                    ctx.AddFailure(new ValidationFailure(nameof(Query.Password), "Password must contain at least one special character"));
                }
            });
        }
    }

    /// <inheritdoc />
    public async Task<QueryResponse<IdentityResult>> Handle(Query request, CancellationToken cancellationToken)
    {
        if (!userManager.SupportsUserEmail)
        {
            throw new BonesException($"{nameof(RegisterUser)} requires a user store with email support.");
        }

        if (string.IsNullOrEmpty(request.Email) || !await request.Email.IsValidEmailAsync(cancellationToken))
        {
            return IdentityResult.Failed(userManager.ErrorDescriber.InvalidEmail(request.Email));
        }

        BonesUser user = new();

        await userManager.SetUserNameAsync(user, request.Email);
        await userManager.SetEmailAsync(user, request.Email);

        IdentityResult result = await userManager.CreateAsync(user, request.Password);
        BonesUser? createdUser = await sender.Send(new GetUserByEmailDb.Query(request.Email), cancellationToken);

        if (result.Succeeded && createdUser != null)
        {
            await sender.Send(new QueueConfirmationEmail.Command(user, request.Email), cancellationToken);
            await sender.Send(new AddAccountAuditDb.Command(
                createdUser,
                Database.DbSets.Audits.AccountAudit.Actions.Create,
                createdUser,
                "Registered"), cancellationToken);
        }

        return result;
    }
}
