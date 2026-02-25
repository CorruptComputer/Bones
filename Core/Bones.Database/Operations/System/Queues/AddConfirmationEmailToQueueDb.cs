using Bones.Database.DbSets.System.Queues;
using Bones.Shared.Extensions;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Bones.Database.Operations.System.Queues;

/// <inheritdoc />
public sealed class AddConfirmationEmailToQueueDb(BonesDbContext dbContext) : IRequestHandler<AddConfirmationEmailToQueueDb.Command, CommandResponse>
{
    /// <summary>
    ///   Adds an email confirmation to the queue
    /// </summary>
    /// <param name="EmailTo">The email being confirmed and the email will be sent to</param>
    /// <param name="ConfirmationLink">The link for them to click</param>
    public sealed record Command(string EmailTo, string ConfirmationLink) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.EmailTo).NotEmpty().EmailAddress().CustomAsync(async (email, ctx, cancel) =>
            {
                if (!await email.IsValidEmailAsync(cancel))
                {
                    ctx.AddFailure(new ValidationFailure(nameof(Command.EmailTo), "Email domain is invalid"));
                }
            });

            RuleFor(x => x.ConfirmationLink).NotEmpty().Custom((str, ctx) =>
            {
                try
                {
                    _ = new Uri(str);
                }
                catch (UriFormatException)
                {
                    ctx.AddFailure(new ValidationFailure(nameof(Command.ConfirmationLink), "Confirmation link is invalid"));
                }
            });
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(AddConfirmationEmailToQueueDb.Command request, CancellationToken cancellationToken)
    {
        if (!await request.EmailTo.IsValidEmailAsync(cancellationToken))
        {
            return CommandResponse.Fail("Invalid email address");
        }

        if (await dbContext.ConfirmationEmailQueue.AnyAsync(x => x.EmailTo == request.EmailTo, cancellationToken))
        {
            return CommandResponse.Fail("Email address already in queue for confirmation");
        }

        EntityEntry<ConfirmationEmailQueue> created = await dbContext.ConfirmationEmailQueue.AddAsync(new()
        {
            EmailTo = request.EmailTo,
            ConfirmationLink = request.ConfirmationLink
        }, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass(nameof(ConfirmationEmailQueue), created.Entity.Id);
    }
}