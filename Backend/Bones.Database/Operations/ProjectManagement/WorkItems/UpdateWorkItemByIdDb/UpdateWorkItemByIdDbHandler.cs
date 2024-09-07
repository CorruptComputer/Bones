using Bones.Database.DbSets.ProjectManagement;
using Bones.Database.DbSets.ProjectManagement.WorkItems;

namespace Bones.Database.Operations.ProjectManagement.WorkItems.UpdateWorkItemByIdDb;

internal sealed class UpdateWorkItemByIdDbHandler(BonesDbContext dbContext) : IRequestHandler<UpdateWorkItemByIdDbCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(UpdateWorkItemByIdDbCommand request, CancellationToken cancellationToken)
    {
        WorkItem? item = await dbContext.WorkItems.FirstOrDefaultAsync(p => p.Id == request.WorkItemId, cancellationToken);
        if (item == null)
        {
            return CommandResponse.Fail("Invalid ItemId.");
        }

        Queue? queue = await dbContext.Queues.FirstOrDefaultAsync(q => q.Id == request.QueueId, cancellationToken);
        if (queue == null)
        {
            return CommandResponse.Fail("Invalid QueueId.");
        }

        List<Tag> tags = [];
        foreach (Guid tagId in request.TagIds)
        {
            Tag? tag = await dbContext.Tags.FirstOrDefaultAsync(t => t.Id == tagId, cancellationToken);
            if (tag == null)
            {
                return CommandResponse.Fail($"Invalid TagId: {tagId}");
            }
            tags.Add(tag);
        }

        item.Name = request.Name;
        item.Queue = queue;
        item.AddedToQueueDateTime = DateTimeOffset.Now;
        item.Tags = tags;

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass();
    }
}