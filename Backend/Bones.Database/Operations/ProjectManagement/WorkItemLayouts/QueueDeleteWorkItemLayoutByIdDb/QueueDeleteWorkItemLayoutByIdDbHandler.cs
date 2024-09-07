using Bones.Database.DbSets.ProjectManagement.WorkItemLayouts;

namespace Bones.Database.Operations.ProjectManagement.WorkItemLayouts.QueueDeleteWorkItemLayoutByIdDb;

internal sealed class QueueDeleteWorkItemLayoutByIdDbHandler(BonesDbContext dbContext) : IRequestHandler<QueueDeleteWorkItemLayoutByIdDbCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(QueueDeleteWorkItemLayoutByIdDbCommand request, CancellationToken cancellationToken)
    {
        WorkItemLayout? layout = await dbContext.WorkItemLayouts.Include(layout => layout.Versions).FirstOrDefaultAsync(p => p.Id == request.LayoutId, cancellationToken);
        if (layout == null)
        {
            return CommandResponse.Fail("Invalid InitiativeId.");
        }

        foreach (WorkItemLayoutVersion version in layout.Versions)
        {
            // TODO: send it
            version.DeleteFlag = true;
        }

        layout.DeleteFlag = true;

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass();
    }
}