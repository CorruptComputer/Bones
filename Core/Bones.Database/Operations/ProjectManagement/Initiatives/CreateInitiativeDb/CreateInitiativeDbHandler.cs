using Bones.Database.DbSets.ProjectManagement;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Bones.Database.Operations.ProjectManagement.Initiatives.CreateInitiativeDb;

/// <inheritdoc />
internal sealed class CreateInitiativeDbHandler(BonesDbContext dbContext) : IRequestHandler<CreateInitiativeDbCommand, CommandResponse>
{
    /// <inheritdoc />
    public async Task<CommandResponse> Handle(CreateInitiativeDbCommand request, CancellationToken cancellationToken)
    {
        Project? project = await dbContext.Projects.FirstOrDefaultAsync(p => p.Id == request.ProjectId, cancellationToken);
        if (project == null)
        {
            return CommandResponse.Fail("Invalid ProjectId.");
        }

        EntityEntry<Initiative> created = await dbContext.Initiatives.AddAsync(new()
        {
            Name = request.Name,
            Project = project
        }, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass(created.Entity.Id);
    }
}