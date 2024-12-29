using Bones.Database.DbSets.AccountManagement;
using Bones.Shared;
using Bones.Shared.Exceptions;
using Bones.Shared.Extensions;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;

namespace Bones.Logic.Features.Accounts;

/// <summary>
///   Backend request for registering a new user.
/// </summary>
/// <param name="Email">Their email address</param>
/// <param name="Password">Their desired password</param>
public sealed record RegisterUserQuery(string Email, string Password) : IRequest<QueryResponse<IdentityResult>>;

internal sealed class RegisterUserQueryValidator : AbstractValidator<RegisterUserQuery>
{
    public override Task<ValidationResult> ValidateAsync(ValidationContext<RegisterUserQuery> context, CancellationToken cancellation = default)
    {
        RuleFor(request => request.Email).NotNull().NotEmpty().EmailAddress().CustomAsync(async (email, ctx, cancel) =>
        {
            if (!await email.IsValidEmailAsync(cancel))
            {
                ctx.AddFailure(new ValidationFailure(nameof(RegisterUserQuery.Email), "Email domain is invalid"));
            }
        });

        RuleFor(request => request.Password).NotNull().MinimumLength(8).Custom((password, ctx) =>
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return;
            }

            if (!StandardRegexes.PasswordContainsUpper().IsMatch(password))
            {
                ctx.AddFailure(new ValidationFailure(nameof(RegisterUserQuery.Password), "Password must contain at least one capital letter"));
            }

            if (!StandardRegexes.PasswordContainsLower().IsMatch(password))
            {
                ctx.AddFailure(new ValidationFailure(nameof(RegisterUserQuery.Password), "Password must contain at least one lowercase letter"));
            }

            if (!StandardRegexes.PasswordContainsNumber().IsMatch(password))
            {
                ctx.AddFailure(new ValidationFailure(nameof(RegisterUserQuery.Password), "Password must contain at least one digit"));
            }

            if (!StandardRegexes.PasswordContainsSpecial().IsMatch(password))
            {
                ctx.AddFailure(new ValidationFailure(nameof(RegisterUserQuery.Password), "Password must contain at least one special character"));
            }
        });

        return base.ValidateAsync(context, cancellation);
    }
}

internal class RegisterUserHandler(UserManager<BonesUser> userManager, ISender sender) : IRequestHandler<RegisterUserQuery, QueryResponse<IdentityResult>>
{
    public async Task<QueryResponse<IdentityResult>> Handle(RegisterUserQuery request, CancellationToken cancellationToken)
    {
        if (!userManager.SupportsUserEmail)
        {
            throw new BonesException($"{nameof(RegisterUserHandler)} requires a user store with email support.");
        }

        if (string.IsNullOrEmpty(request.Email) || !await request.Email.IsValidEmailAsync(cancellationToken))
        {
            return IdentityResult.Failed(userManager.ErrorDescriber.InvalidEmail(request.Email));
        }

        BonesUser user = new();

        await userManager.SetUserNameAsync(user, request.Email);
        await userManager.SetEmailAsync(user, request.Email);

        IdentityResult result = await userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
        {
            await sender.Send(new QueueConfirmationEmailCommand(user, request.Email), cancellationToken);
        }

        return result;
    }
}