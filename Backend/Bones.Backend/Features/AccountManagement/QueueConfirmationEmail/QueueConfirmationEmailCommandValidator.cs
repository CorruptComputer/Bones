using Bones.Shared.Extensions;
using FluentValidation.Results;

namespace Bones.Backend.Features.AccountManagement.QueueConfirmationEmail;

internal class QueueConfirmationEmailCommandValidator : AbstractValidator<QueueConfirmationEmailCommand>
{
    public override Task<ValidationResult> ValidateAsync(ValidationContext<QueueConfirmationEmailCommand> context, CancellationToken cancellation = new())
    {
        RuleFor(x => x.Email).NotNull().NotEmpty().EmailAddress().CustomAsync(async (email, ctx, cancel) =>
        {
            if (!await email.IsValidEmailAsync(cancel))
            {
                ctx.AddFailure(new ValidationFailure(nameof(QueueConfirmationEmailCommand.Email), "Email domain is invalid"));
            }
        });

        return base.ValidateAsync(context, cancellation);
    }
}