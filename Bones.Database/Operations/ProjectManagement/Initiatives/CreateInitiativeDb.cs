using Bones.Database.DbSets.ProjectManagement.Initiatives;
using Bones.Database.DbSets.ProjectManagement.Projects;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Bones.Database.Operations.ProjectManagement.Initiatives;

public sealed class CreateInitiativeDb(BonesDbContext dbContext) : IRequestHandler<CreateInitiativeDb.Command, CommandResponse>
{
    /// <summary>
    ///     DB Command for creating an Initiative.
    /// </summary>
    /// <param name="Name">Name of the initiative</param>
    /// <param name="ProjectId">Internal ID of the project</param>
    public record Command(string Name, Guid ProjectId) : IValidatableRequest<CommandResponse>
    {
        /// <inheritdoc />
        public bool IsRequestValid()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                return false;
            }

            if (ProjectId == Guid.Empty)
            {
                return false;
            }

            return true;
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        Project? project = await dbContext.Projects.FirstOrDefaultAsync(p => p.Id == request.ProjectId, cancellationToken);
        if (project == null)
        {
            return new()
            {
                Success = false,
                FailureReason = "Invalid ProjectId."
            };
        }

        EntityEntry<Initiative> created = await dbContext.Initiatives.AddAsync(new()
        {
            Name = request.Name,
            Project = project
        }, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new()
        {
            Success = true,
            Id = created.Entity.Id
        };
    }
}