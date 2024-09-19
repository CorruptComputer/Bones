using Bones.Shared.Extensions;
using FluentValidation.Results;

namespace Bones.Database.Operations.SystemQueues.AddForgotPasswordEmailToQueueDb;

internal sealed class AddForgotPasswordEmailToQueueDbCommandValidator : AbstractValidator<AddForgotPasswordEmailToQueueDbCommand>
{
    public override Task<ValidationResult> ValidateAsync(ValidationContext<AddForgotPasswordEmailToQueueDbCommand> context, CancellationToken cancellation = new())
    {
        RuleFor(x => x.EmailTo).NotNull().NotEmpty().EmailAddress().CustomAsync(async (email, ctx, cancel) =>
        {
            if (!await email.IsValidEmailAsync(cancel))
            {
                ctx.AddFailure(new ValidationFailure(nameof(AddForgotPasswordEmailToQueueDbCommand.EmailTo), "Email domain is invalid"));
            }
        });

        RuleFor(x => x.PasswordResetLink).NotNull().NotEmpty().Custom((str, ctx) =>
        {
            try
            {
                _ = new Uri(str);
            }
            catch (UriFormatException)
            {
                ctx.AddFailure(new ValidationFailure(nameof(AddForgotPasswordEmailToQueueDbCommand.PasswordResetLink), "Reset link is invalid"));
            }
        });

        return base.ValidateAsync(context, cancellation);
    }
}