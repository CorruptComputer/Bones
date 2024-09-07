using Bones.Database.DbSets.ProjectManagement.WorkItemLayouts;

namespace Bones.Database.Operations.ProjectManagement.WorkItemLayouts.UpdateWorkItemLayoutByIdDb;

internal sealed class UpdateWorkItemLayoutByIdDbHandler(BonesDbContext dbContext) : IRequestHandler<UpdateWorkItemLayoutByIdDbCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(UpdateWorkItemLayoutByIdDbCommand request, CancellationToken cancellationToken)
    {
        WorkItemLayout? layout = await dbContext.WorkItemLayouts.FirstOrDefaultAsync(p => p.Id == request.LayoutId, cancellationToken);
        if (layout == null)
        {
            return CommandResponse.Fail("Invalid LayoutId.");
        }

        layout.Name = request.NewName;

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass();
    }
}