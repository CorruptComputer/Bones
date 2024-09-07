using Bones.Database.DbSets.ProjectManagement.WorkItems;

namespace Bones.Database.Operations.ProjectManagement.WorkItems.QueueDeleteWorkItemVersionByIdDb;

internal sealed class QueueDeleteWorkItemVersionByIdDbHandler(BonesDbContext dbContext) : IRequestHandler<QueueDeleteWorkItemVersionByIdDbCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(QueueDeleteWorkItemVersionByIdDbCommand request, CancellationToken cancellationToken)
    {
        WorkItemVersion? workItemVersion = await dbContext.WorkItemVersions
            .Include(itemVersion => itemVersion.Values)
            .FirstOrDefaultAsync(p => p.Id == request.WorkItemVersionId, cancellationToken);

        if (workItemVersion == null)
        {
            return CommandResponse.Fail("Invalid ItemVersionId.");
        }

        foreach (WorkItemValue value in workItemVersion.Values)
        {
            value.DeleteFlag = true;
        }

        workItemVersion.DeleteFlag = true;

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass();
    }
}