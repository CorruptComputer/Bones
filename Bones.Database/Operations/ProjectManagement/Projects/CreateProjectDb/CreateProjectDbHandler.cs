using Bones.Database.DbSets.ProjectManagement.Projects;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Bones.Database.Operations.ProjectManagement.Projects.CreateProjectDb;

public sealed class CreateProjectDbHandler(BonesDbContext dbContext) : IRequestHandler<CreateProjectDbCommand, CommandResponse>
{
    /// <inheritdoc />
    public async Task<CommandResponse> Handle(CreateProjectDbCommand request, CancellationToken cancellationToken)
    {
        EntityEntry<Project> created = await dbContext.Projects.AddAsync(new()
        {
            Name = request.Name,
            OwningUser = request.RequestingUser
        }, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new()
        {
            Success = true,
            Id = created.Entity.Id
        };
    }
}