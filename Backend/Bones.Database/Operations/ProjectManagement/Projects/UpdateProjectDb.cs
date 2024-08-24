using Bones.Database.DbSets.ProjectManagement;
using Bones.Shared.Backend.Models;

namespace Bones.Database.Operations.ProjectManagement.Projects;

public sealed class UpdateProjectDb(BonesDbContext dbContext) : IRequestHandler<UpdateProjectDb.Command, CommandResponse>
{
    /// <summary>
    ///     DB Command for updating a Project.
    /// </summary>
    /// <param name="ProjectId">Internal ID of the project</param>
    /// <param name="Name">The new name of the project</param>
    public record Command(Guid ProjectId, string Name) : IValidatableRequest<CommandResponse>
    {
        /// <inheritdoc />
        public (bool valid, string? invalidReason) IsRequestValid()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                return (false, "");
            }

            if (ProjectId == Guid.Empty)
            {
                return (false, "");
            }

            return (true, null);
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

        project.Name = request.Name;
        await dbContext.SaveChangesAsync(cancellationToken);

        return new()
        {
            Success = true
        };
    }
}