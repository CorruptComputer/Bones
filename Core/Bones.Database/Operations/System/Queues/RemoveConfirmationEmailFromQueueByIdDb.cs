using Bones.Database.DbSets.System;

namespace Bones.Database.Operations.System.Queues;

/// <inheritdoc />
public sealed class RemoveConfirmationEmailFromQueueByIdDb(BonesDbContext dbContext) : IRequestHandler<RemoveConfirmationEmailFromQueueByIdDb.Command, CommandResponse>
{
    /// <summary>
    ///   Checks if any confirmation emails are in the queue
    /// </summary>
    public sealed record Command(Guid Id) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty().NotEqual(Guid.Empty);
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

        dbContext.ConfirmationEmailQueue.Remove(queue);
        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass();
    }
}