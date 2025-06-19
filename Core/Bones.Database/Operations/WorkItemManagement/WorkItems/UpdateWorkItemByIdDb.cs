using Bones.Database.DbSets.WorkItemManagement;

namespace Bones.Database.Operations.WorkItemManagement.WorkItems;

/// <inheritdoc />
public sealed class UpdateWorkItemByIdDb(BonesDbContext dbContext) : IRequestHandler<UpdateWorkItemByIdDb.Command, CommandResponse>
{
    /// <summary>
    ///   DB Command for updating an WorkItem.
    /// </summary>
    /// <param name="WorkItemId">Internal ID of the item</param>
    /// <param name="QueueId">Internal ID of the queue</param>
    public record Command(Guid WorkItemId, Guid QueueId) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.WorkItemId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.QueueId).NotNull().NotEqual(Guid.Empty);
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        WorkItem? workItem = await dbContext.WorkItems.Include(workItem => workItem.Item).FirstOrDefaultAsync(p => p.Id == request.WorkItemId, cancellationToken);
        if (workItem == null)
        {
            return CommandResponse.Fail("Invalid WorkItem ID.");
        }

        WorkItemQueue? queue = await dbContext.WorkItemQueues.FirstOrDefaultAsync(q => q.Id == request.QueueId, cancellationToken);
        if (queue == null)
        {
            return CommandResponse.Fail("Invalid QueueId.");
        }

        workItem.WorkItemQueue = queue;
        workItem.AddedToQueueDateTime = DateTimeOffset.Now;

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass();
    }
}