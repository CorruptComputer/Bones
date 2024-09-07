using Bones.Database.DbSets.ProjectManagement;
using Bones.Database.DbSets.ProjectManagement.WorkItemLayouts;
using Bones.Database.Operations.ProjectManagement.Projects.GetProjectByIdDb;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Bones.Database.Operations.ProjectManagement.WorkItemLayouts.CreateWorkItemLayoutDb;

internal sealed class CreateWorkItemLayoutDbHandler(BonesDbContext dbContext, ISender sender) : IRequestHandler<CreateWorkItemLayoutDbCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(CreateWorkItemLayoutDbCommand request, CancellationToken cancellationToken)
    {
        Project? project = await sender.Send(new GetProjectByIdDbQuery(request.ProjectId), cancellationToken);

        if (project == null)
        {
            return CommandResponse.Fail("Project not found");
        }
        
        WorkItemLayout workItemLayout = new()
        {
            Name = request.Name,
            Project = project
        };
        
        EntityEntry<WorkItemLayout> createdLayout = await dbContext.WorkItemLayouts.AddAsync(workItemLayout, cancellationToken);
        WorkItemLayoutVersion initialVersion = new()
        {
            Version = 0,
            WorkItemLayout = createdLayout.Entity
        };

        createdLayout.Entity.Versions.Add(initialVersion);
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return CommandResponse.Pass(createdLayout.Entity.Id);
    }
}