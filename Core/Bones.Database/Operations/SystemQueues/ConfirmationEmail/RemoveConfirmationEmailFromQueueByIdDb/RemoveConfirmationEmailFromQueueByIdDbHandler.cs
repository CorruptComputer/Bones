using Bones.Database.DbSets.SystemQueues;

namespace Bones.Database.Operations.SystemQueues.ConfirmationEmail.RemoveConfirmationEmailFromQueueByIdDb;

/// <summary>
///   Checks if any confirmation emails are in the queue
/// </summary>
public sealed record RemoveConfirmationEmailFromQueueByIdDbCommand(Guid Id) : IRequest<CommandResponse>;

internal sealed class RemoveConfirmationEmailFromQueueByIdDbHandler(BonesDbContext dbContext) : IRequestHandler<RemoveConfirmationEmailFromQueueByIdDbCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(RemoveConfirmationEmailFromQueueByIdDbCommand request, CancellationToken cancellationToken)
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