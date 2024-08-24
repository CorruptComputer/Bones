using Bones.Database.DbSets.ProjectManagement;
using Bones.Database.Operations.ProjectManagement.Items;
using Bones.Shared.Backend.Models;

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
        public (bool valid, string? invalidReason) IsRequestValid()
        {
            if (QueueId == Guid.Empty)
            {
                return (false, "Queue id is required.");
            }

            return (true, null);
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

        foreach (WorkItem item in queue.Items)
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