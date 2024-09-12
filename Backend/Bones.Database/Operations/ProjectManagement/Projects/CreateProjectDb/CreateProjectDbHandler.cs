using Bones.Database.DbSets.ProjectManagement;
using Bones.Shared.Backend.Enums;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Bones.Database.Operations.ProjectManagement.Projects.CreateProjectDb;

internal sealed class CreateProjectDbHandler(BonesDbContext dbContext) : IRequestHandler<CreateProjectDbCommand, CommandResponse>
{
    /// <inheritdoc />
    public async Task<CommandResponse> Handle(CreateProjectDbCommand request, CancellationToken cancellationToken)
    {
        Project project = new()
        {
            Name = request.Name
        };

        if (request.Organization == null)
        {
            project.OwnerType = OwnershipType.User;
            project.OwningUser = request.RequestingUser;
        }
        else
        {
            project.OwnerType = OwnershipType.Organization;
            project.OwningOrganization = request.Organization;
        }


        EntityEntry<Project> created = await dbContext.Projects.AddAsync(project, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass(created.Entity.Id);
    }
}