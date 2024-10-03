using Bones.Database.DbSets.WorkItemManagement;
using Bones.Database.Operations.WorkItemManagement.WorkItems.QueueDeleteWorkItemByIdDb;

namespace Bones.Database.Operations.WorkItemManagement.WorkItemQueues.QueueDeleteQueueByIdDb;

internal sealed class QueueDeleteQueueByIdDbHandler(BonesDbContext dbContext, ISender sender) : IRequestHandler<QueueDeleteQueueByIdDbCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(QueueDeleteQueueByIdDbCommand request, CancellationToken cancellationToken)
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
            await sender.Send(new QueueDeleteWorkItemByIdDbCommand(item.Id), cancellationToken);
        }

        queue.DeleteFlag = true;
        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass();
    }
}