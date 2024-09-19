using Bones.Shared;
using Bones.Shared.Extensions;
using FluentValidation.Results;

namespace Bones.Backend.Features.AccountManagement.RegisterUser;

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