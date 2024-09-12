namespace Bones.Database.Operations.SystemQueues.AddConfirmationEmailToQueue;

internal sealed class AddConfirmationEmailToQueueDbCommandValidator : AbstractValidator<AddConfirmationEmailToQueueDbCommand>
{
    public override Task<ValidationResult> ValidateAsync(ValidationContext<AddConfirmationEmailToQueueDbCommand> context, CancellationToken cancellation = new CancellationToken())
    {
        RuleFor(x => x.EmailTo).NotNull().NotEmpty().EmailAddress();
        RuleFor(x => x.ConfirmationLink).NotNull().NotEmpty().Custom((str, ctx) =>
        {
            try
            {
                _ = new Uri(str);
            }
            catch (UriFormatException)
            {
                ctx.AddFailure("Please provide a valid URL.");
            }
        });

        return base.ValidateAsync(context, cancellation);
    }
}