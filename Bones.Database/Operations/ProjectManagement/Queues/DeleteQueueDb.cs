using Bones.Database.DbSets.ProjectManagement.Items;
using Bones.Database.DbSets.ProjectManagement.Queues;
using Bones.Database.Operations.ProjectManagement.Items;

namespace Bones.Database.Operations.ProjectManagement.Queues;

public sealed class DeleteQueueDb(BonesDbContext dbContext, ISender sender) : IRequestHandler<DeleteQueueDb.Command, CommandResponse>
{
    /// <summary>
    ///     DB Command for deleting a Queue.
    /// </summary>
    /// <param name="QueueId">Internal ID of the queue</param>
    public record Command(Guid QueueId) : IValidatableRequest<CommandResponse>
    {
        /// <inheritdoc />
        public bool IsRequestValid()
        {
            if (QueueId == Guid.Empty)
            {
                return false;
            }

            return true;
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        Queue? queue = await dbContext.Queues
            .Include(queue => queue.Items)
            .FirstOrDefaultAsync(p => p.Id == request.QueueId, cancellationToken);

        if (queue == null)
        {
            return new()
            {
                Success = false,
                FailureReason = "Invalid QueueId."
            };
        }

        foreach (Item item in queue.Items)
        {
            // TODO: Might want to eventually add the ability to move these to a different queue instead
            await sender.Send(new DeleteItemDb.Command(item.Id), cancellationToken);
        }

        queue.DeleteFlag = true;
        await dbContext.SaveChangesAsync(cancellationToken);

        return new()
        {
            Success = true
        };
    }
}