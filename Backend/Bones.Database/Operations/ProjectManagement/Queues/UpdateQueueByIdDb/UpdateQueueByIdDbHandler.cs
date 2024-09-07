using Bones.Database.DbSets.ProjectManagement;

namespace Bones.Database.Operations.ProjectManagement.Queues.UpdateQueueByIdDb;

internal sealed class UpdateQueueByIdDbHandler(BonesDbContext dbContext) : IRequestHandler<UpdateQueueByIdDbCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(UpdateQueueByIdDbCommand request, CancellationToken cancellationToken)
    {
        Queue? queue = await dbContext.Queues.FirstOrDefaultAsync(p => p.Id == request.QueueId, cancellationToken);
        if (queue == null)
        {
            return CommandResponse.Fail("Invalid QueueId.");
        }

        queue.Name = request.NewName;

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass();
    }
}