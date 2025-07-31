using Bones.Database.DbSets.System;
using Bones.Shared.Extensions;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Bones.Database.Operations.System.Queues;

/// <inheritdoc />
public sealed class AddForgotPasswordEmailToQueueDb(BonesDbContext dbContext) : IRequestHandler<AddForgotPasswordEmailToQueueDb.Command, CommandResponse>
{
    /// <summary>
    ///   Adds an email confirmation to the queue
    /// </summary>
    /// <param name="EmailTo">The email being confirmed and the email will be sent to</param>
    /// <param name="PasswordResetLink">The link for them to click</param>
    public sealed record Command(string EmailTo, string PasswordResetLink) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.EmailTo).NotNull().NotEmpty().EmailAddress().CustomAsync(async (email, ctx, cancel) =>
            {
                if (!await email.IsValidEmailAsync(cancel))
                {
                    ctx.AddFailure(new ValidationFailure(nameof(Command.EmailTo), "Email domain is invalid"));
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
                    ctx.AddFailure(new ValidationFailure(nameof(Command.PasswordResetLink), "Reset link is invalid"));
                }
            });
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        if (!await request.EmailTo.IsValidEmailAsync(cancellationToken))
        {
            return CommandResponse.Fail("Invalid email address");
        }

        if (await dbContext.ForgotPasswordEmailQueue.AnyAsync(x => x.EmailTo == request.EmailTo, cancellationToken))
        {
            return CommandResponse.Fail("Email address already in queue for reset");
        }

        EntityEntry<ForgotPasswordEmailQueue> created = await dbContext.ForgotPasswordEmailQueue.AddAsync(new()
        {
            EmailTo = request.EmailTo,
            PasswordResetLink = request.PasswordResetLink
        }, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass(nameof(ForgotPasswordEmailQueue), created.Entity.Id);
    }
}