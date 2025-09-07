using Bones.Database.DbSets.ProjectManagement;
using Bones.Database.DbSets.WorkItemManagement;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Bones.Database.Operations.WorkItemManagement.WorkItemQueues;

/// <inheritdoc />
public sealed class MoveWorkItemToQueueDb(BonesDbContext dbContext) : IRequestHandler<MoveWorkItemToQueueDb.Command, CommandResponse>
{
    /// <summary>
    ///   DB Command for creating a Queue.
    /// </summary>
    /// <param name="WorkItemId"></param>
    /// <param name="WorkItemQueueId"></param>
    public sealed record Command(Guid WorkItemId, Guid WorkItemQueueId) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.WorkItemId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.WorkItemQueueId).NotNull().NotEqual(Guid.Empty);
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        WorkItem? workItem = await dbContext.WorkItems.FindAsync([request.WorkItemId], cancellationToken);
        WorkItemQueue? workItemQueue = await dbContext.WorkItemQueues.FindAsync([request.WorkItemQueueId], cancellationToken);

        if (workItem is null || workItemQueue is null)
        {
            return CommandResponse.Fail("Invalid Work Item or Work Item Queue ID.");
        }

        workItem.WorkItemQueue = workItemQueue;
        workItem.AddedToQueueDateTime = DateTimeOffset.UtcNow;

        dbContext.WorkItems.Update(workItem);

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass();
    }
}
