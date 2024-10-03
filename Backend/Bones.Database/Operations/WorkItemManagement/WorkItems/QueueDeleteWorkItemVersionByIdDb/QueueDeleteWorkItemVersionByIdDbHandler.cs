using Bones.Database.DbSets.GenericItems.Items;

namespace Bones.Database.Operations.WorkItemManagement.WorkItems.QueueDeleteWorkItemVersionByIdDb;

internal sealed class QueueDeleteWorkItemVersionByIdDbHandler(BonesDbContext dbContext) : IRequestHandler<QueueDeleteWorkItemVersionByIdDbCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(QueueDeleteWorkItemVersionByIdDbCommand request, CancellationToken cancellationToken)
    {
        ItemVersion? workItemVersion = await dbContext.ItemVersions
            .Include(itemVersion => itemVersion.Values)
            .FirstOrDefaultAsync(p => p.Id == request.WorkItemVersionId, cancellationToken);

        if (workItemVersion == null)
        {
            return CommandResponse.Fail("Invalid ItemVersionId.");
        }

        foreach (ItemValue value in workItemVersion.Values)
        {
            value.DeleteFlag = true;
        }

        workItemVersion.DeleteFlag = true;

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass();
    }
}