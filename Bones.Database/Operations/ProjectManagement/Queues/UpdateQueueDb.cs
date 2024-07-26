using Bones.Database.DbSets.ProjectManagement.Queues;

namespace Bones.Database.Operations.ProjectManagement.Queues;

public sealed class UpdateQueueDb(BonesDbContext dbContext) : IRequestHandler<UpdateQueueDb.Command, CommandResponse>
{
    /// <summary>
    ///     DB Command for updating a Queue.
    /// </summary>
    /// <param name="QueueId">Internal ID of the queue</param>
    /// <param name="Name">The new name of the queue</param>
    public record Command(Guid QueueId, string Name) : IValidatableRequest<CommandResponse>
    {
        /// <inheritdoc />
        public bool IsRequestValid()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                return false;
            }

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
        Queue? queue = await dbContext.Queues.FirstOrDefaultAsync(p => p.Id == request.QueueId, cancellationToken);
        if (queue == null)
        {
            return new()
            {
                Success = false,
                FailureReason = "Invalid QueueId."
            };
        }

        queue.Name = request.Name;

        await dbContext.SaveChangesAsync(cancellationToken);

        return new()
        {
            Success = true
        };
    }
}