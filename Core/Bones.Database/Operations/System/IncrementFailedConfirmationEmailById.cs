using Bones.Database.DbSets.System;

namespace Bones.Database.Operations.System;

/// <inheritdoc />
public sealed class IncrementFailedConfirmationEmailById(BonesDbContext dbContext) : IRequestHandler<IncrementFailedConfirmationEmailById.Command, CommandResponse>
{
    /// <summary>
    ///   Checks if any confirmation emails are in the queue
    /// </summary>
    public sealed record Command(Guid Id, string FailureReason) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty().NotEqual(Guid.Empty);
            RuleFor(x => x.FailureReason).NotEmpty();
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        ConfirmationEmailQueue? queue = await dbContext.ConfirmationEmailQueue.FindAsync([request.Id], cancellationToken);

        if (queue == null)
        {
            return CommandResponse.Fail("Could not find confirmation email in queue.");
        }

        queue.LastTry = DateTimeOffset.Now;
        queue.RetryCount++;
        queue.FailureReasons.Add(request.FailureReason);

        if (queue.RetryCount >= 5)
        {
            await dbContext.ConfirmationEmailDeadQueue.AddAsync(ConfirmationEmailDeadQueue.FromConfirmationEmailQueue(queue), cancellationToken);
            dbContext.ConfirmationEmailQueue.Remove(queue);
            Log.Warning("Moving email to Confirmation Email Dead Queue: {Id}", queue.Id);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass();
    }
}