using Bones.Database.DbSets.WorkItemManagement;
using Bones.Database.Operations.WorkItemManagement.WorkItems;

namespace Bones.Database.Operations.WorkItemManagement.WorkItemQueues;

/// <inheritdoc />
public sealed class QueueDeleteQueueByIdDb(BonesDbContext dbContext, ISender sender) : IRequestHandler<QueueDeleteQueueByIdDb.Command, CommandResponse>
{
    /// <summary>
    ///   DB Command for deleting a Queue.
    /// </summary>
    /// <param name="QueueId">Internal ID of the queue</param>
    public sealed record Command(Guid QueueId) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.QueueId).NotNull().NotEqual(Guid.Empty);
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        WorkItemQueue? queue = await dbContext.WorkItemQueues
            .Include(queue => queue.WorkItems)
            .FirstOrDefaultAsync(p => p.Id == request.QueueId, cancellationToken);

        if (queue == null)
        {
            return CommandResponse.Fail("Invalid QueueId.");
        }

        foreach (WorkItem item in queue.WorkItems)
        {
            // TODO: Might want to eventually add the ability to move these to a different queue instead
            await sender.Send(new QueueDeleteWorkItemByIdDb.Command(item.Id), cancellationToken);
        }

        queue.DeleteFlag = true;
        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass();
    }
}