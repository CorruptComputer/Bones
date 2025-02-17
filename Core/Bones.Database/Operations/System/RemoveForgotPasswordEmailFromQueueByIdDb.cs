using Bones.Database.DbSets.System;

namespace Bones.Database.Operations.System;

/// <inheritdoc />
public sealed class RemoveForgotPasswordEmailFromQueueByIdDb(BonesDbContext dbContext) : IRequestHandler<RemoveForgotPasswordEmailFromQueueByIdDb.Command, CommandResponse>
{
    /// <summary>
    ///   Checks if any confirmation emails are in the queue
    /// </summary>
    public sealed record Command(Guid Id) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
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