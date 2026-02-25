using Bones.Database.DbSets.System.Queues;

namespace Bones.Database.Operations.System.Queues;

/// <inheritdoc />
public sealed class IncrementFailedForgotPasswordEmailById(BonesDbContext dbContext) : IRequestHandler<IncrementFailedForgotPasswordEmailById.Command, CommandResponse>
{
    /// <summary>
    ///   Checks if any confirmation emails are in the queue
    /// </summary>
    public sealed record Command(Guid Id, string FailureReason) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        ForgotPasswordEmailQueue? queue = await dbContext.ForgotPasswordEmailQueue.FindAsync([request.Id], cancellationToken);

        if (queue == null)
        {
            return CommandResponse.Fail("Could not find forgot password email in queue.");
        }

        queue.LastTry = DateTimeOffset.Now;
        queue.RetryCount++;
        queue.FailureReasons.Add(request.FailureReason);

        if (queue.RetryCount >= 5)
        {
            await dbContext.ForgotPasswordEmailDeadQueue.AddAsync(ForgotPasswordEmailDeadQueue.FromForgotPasswordEmailQueue(queue), cancellationToken);
            dbContext.ForgotPasswordEmailQueue.Remove(queue);
            Log.Warning("Moving email to forgot password email dead queue: {Id}", queue.Id);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass();
    }
}