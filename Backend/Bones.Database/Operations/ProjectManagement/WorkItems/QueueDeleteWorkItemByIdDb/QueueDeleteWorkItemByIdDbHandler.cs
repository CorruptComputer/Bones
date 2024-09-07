using Bones.Database.DbSets.ProjectManagement.WorkItems;
using Bones.Database.Operations.ProjectManagement.WorkItems.QueueDeleteWorkItemVersionByIdDb;

namespace Bones.Database.Operations.ProjectManagement.WorkItems.QueueDeleteWorkItemByIdDb;

internal sealed class QueueDeleteWorkItemByIdDbHandler(BonesDbContext dbContext, ISender sender) : IRequestHandler<QueueDeleteWorkItemByIdDbCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(QueueDeleteWorkItemByIdDbCommand request, CancellationToken cancellationToken)
    {
        WorkItem? workItem = await dbContext.WorkItems
            .Include(item => item.Versions)
            .ThenInclude(itemVersion => itemVersion.Values)
            .FirstOrDefaultAsync(p => p.Id == request.WorkItemId, cancellationToken);

        if (workItem == null)
        {
            return CommandResponse.Fail("Invalid ItemId.");
        }

        foreach (WorkItemVersion version in workItem.Versions)
        {
            await sender.Send(new QueueDeleteWorkItemVersionByIdDbCommand(version.Id), cancellationToken);
        }

        workItem.DeleteFlag = true;

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass();
    }
}