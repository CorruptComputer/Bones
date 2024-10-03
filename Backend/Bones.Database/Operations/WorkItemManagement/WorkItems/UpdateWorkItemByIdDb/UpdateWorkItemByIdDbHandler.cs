using Bones.Database.DbSets.WorkItemManagement;

namespace Bones.Database.Operations.WorkItemManagement.WorkItems.UpdateWorkItemByIdDb;

internal sealed class UpdateWorkItemByIdDbHandler(BonesDbContext dbContext) : IRequestHandler<UpdateWorkItemByIdDbCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(UpdateWorkItemByIdDbCommand request, CancellationToken cancellationToken)
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

        workItem.Item.Name = request.Name;
        workItem.WorkItemQueue = queue;
        workItem.AddedToQueueDateTime = DateTimeOffset.Now;

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass();
    }
}