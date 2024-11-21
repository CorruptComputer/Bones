using Bones.Database.DbSets.SystemQueues;

namespace Bones.Database.Operations.SystemQueues.ConfirmationEmail.IncrementFailedConfirmationEmailById;

/// <summary>
///   Checks if any confirmation emails are in the queue
/// </summary>
public sealed record IncrementFailedConfirmationEmailByIdCommand(Guid Id, string FailureReason) : IRequest<CommandResponse>;

internal sealed class IncrementFailedConfirmationEmailByIdHandler(BonesDbContext dbContext) : IRequestHandler<IncrementFailedConfirmationEmailByIdCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(IncrementFailedConfirmationEmailByIdCommand request, CancellationToken cancellationToken)
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