using Bones.Database.DbSets.GenericItems.Items;
using Bones.Database.DbSets.WorkItemManagement;
using Bones.Database.Operations.WorkItemManagement.WorkItems.QueueDeleteWorkItemVersionByIdDb;

namespace Bones.Database.Operations.WorkItemManagement.WorkItems.QueueDeleteWorkItemByIdDb;

internal sealed class QueueDeleteWorkItemByIdDbHandler(BonesDbContext dbContext, ISender sender) : IRequestHandler<QueueDeleteWorkItemByIdDbCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(QueueDeleteWorkItemByIdDbCommand request, CancellationToken cancellationToken)
    {
        WorkItem? workItem = await dbContext.WorkItems
            .Include(item => item.Item)
            .ThenInclude(item => item.Versions)
            .FirstOrDefaultAsync(p => p.Id == request.WorkItemId, cancellationToken);

        if (workItem == null)
        {
            return CommandResponse.Fail("Invalid ItemId.");
        }

        foreach (ItemVersion version in workItem.Item.Versions)
        {
            await sender.Send(new QueueDeleteWorkItemVersionByIdDbCommand(version.Id), cancellationToken);
        }

        workItem.DeleteFlag = true;

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass();
    }
}