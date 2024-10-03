using Bones.Database.DbSets.WorkItemManagement;

namespace Bones.Database.Operations.WorkItemManagement.WorkItemQueues.UpdateQueueByIdDb;

internal sealed class UpdateQueueByIdDbHandler(BonesDbContext dbContext) : IRequestHandler<UpdateQueueByIdDbCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(UpdateQueueByIdDbCommand request, CancellationToken cancellationToken)
    {
        WorkItemQueue? queue = await dbContext.WorkItemQueues.FirstOrDefaultAsync(p => p.Id == request.QueueId, cancellationToken);
        if (queue == null)
        {
            return CommandResponse.Fail("Invalid QueueId.");
        }

        queue.Name = request.NewName;

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass();
    }
}