using Bones.Shared.Extensions;
using FluentValidation.Results;

namespace Bones.Backend.Features.AccountManagement.QueueForgotPasswordEmail;

internal class QueueForgotPasswordEmailCommandValidator : AbstractValidator<QueueForgotPasswordEmailCommand>
{
    public override Task<ValidationResult> ValidateAsync(ValidationContext<QueueForgotPasswordEmailCommand> context, CancellationToken cancellation = new())
    {
        RuleFor(x => x.Email).NotNull().NotEmpty().EmailAddress().CustomAsync(async (email, ctx, cancel) =>
        {
            if (!await email.IsValidEmailAsync(cancel))
            {
                ctx.AddFailure(new ValidationFailure(nameof(QueueForgotPasswordEmailCommand.Email), "Email domain is invalid"));
            }
        });

        return base.ValidateAsync(context, cancellation);
    }
}