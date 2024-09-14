using Bones.Shared.Extensions;
using FluentValidation.Results;

namespace Bones.Database.Operations.SystemQueues.AddConfirmationEmailToQueueDb;

internal sealed class AddConfirmationEmailToQueueDbCommandValidator : AbstractValidator<AddConfirmationEmailToQueueDbCommand>
{
    public override Task<ValidationResult> ValidateAsync(ValidationContext<AddConfirmationEmailToQueueDbCommand> context, CancellationToken cancellation = new CancellationToken())
    {
        RuleFor(x => x.EmailTo).NotNull().NotEmpty().EmailAddress().CustomAsync(async (email, ctx, cancel) =>
        {
            if (!await email.IsValidEmailAsync(cancel))
            {
                ctx.AddFailure(new ValidationFailure(nameof(AddConfirmationEmailToQueueDbCommand.EmailTo), "Email domain is invalid"));
            }
        });

        RuleFor(x => x.ConfirmationLink).NotNull().NotEmpty().Custom((str, ctx) =>
        {
            try
            {
                _ = new Uri(str);
            }
            catch (UriFormatException)
            {
                ctx.AddFailure(new ValidationFailure(nameof(AddConfirmationEmailToQueueDbCommand.ConfirmationLink), "Confirmation link is invalid"));
            }
        });

        return base.ValidateAsync(context, cancellation);
    }
}