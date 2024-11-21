using Bones.Database.DbSets.GenericItems.GenericItemLayouts;
using Bones.Database.DbSets.WorkItemManagement;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Bones.Database.Operations.WorkItemManagement.WorkItems.CreateWorkItemDb;

internal sealed class CreateWorkItemDbHandler(BonesDbContext dbContext) : IRequestHandler<CreateWorkItemDbCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(CreateWorkItemDbCommand request, CancellationToken cancellationToken)
    {
        WorkItemQueue? queue = await dbContext.WorkItemQueues.FirstOrDefaultAsync(q => q.Id == request.QueueId, cancellationToken);
        if (queue == null)
        {
            return CommandResponse.Fail("Invalid Queue ID.");
        }

        GenericItemLayout? itemLayout = await dbContext.ItemLayouts.FindAsync([request.ItemLayoutId], cancellationToken);
        if (itemLayout == null)
        {
            return CommandResponse.Fail("Invalid ItemLayout ID.");
        }

        EntityEntry<WorkItem> created = await dbContext.WorkItems.AddAsync(new()
        {
            WorkItemQueue = queue,
            AddedToQueueDateTime = DateTimeOffset.UtcNow,
            Item = new()
            {
                Name = request.Name,
                ProjectId = itemLayout.ProjectId,
                GenericItemLayout = itemLayout
            }
        }, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass(created.Entity.Id);
    }
}