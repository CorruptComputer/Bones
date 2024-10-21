using Bones.Database.DbSets.SystemQueues;

namespace Bones.Database.Operations.SystemQueues.ForgotPassword.RemoveConfirmationEmailFromQueueByIdDb;

/// <summary>
///   Checks if any confirmation emails are in the queue
/// </summary>
public sealed record RemoveForgotPasswordEmailFromQueueByIdDbCommand(Guid Id) : IRequest<CommandResponse>;

internal sealed class RemoveForgotPasswordEmailFromQueueByIdDbHandler(BonesDbContext dbContext) : IRequestHandler<RemoveForgotPasswordEmailFromQueueByIdDbCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(RemoveForgotPasswordEmailFromQueueByIdDbCommand request, CancellationToken cancellationToken)
    {
        ForgotPasswordEmailQueue? queue = await dbContext.ForgotPasswordEmailQueue.FindAsync([request.Id], cancellationToken);

        if (queue == null)
        {
            return CommandResponse.Fail("Could not find forgot password email in queue.");
        }

        dbContext.ForgotPasswordEmailQueue.Remove(queue);
        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass();
    }
}