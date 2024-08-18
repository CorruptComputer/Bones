using Bones.Database.DbSets.ProjectManagement.Initiatives;
using Bones.Database.DbSets.ProjectManagement.Projects;
using Bones.Database.Operations.ProjectManagement.Initiatives;

namespace Bones.Database.Operations.ProjectManagement.Projects;

public sealed class DeleteProjectDb(BonesDbContext dbContext, ISender sender) : IRequestHandler<DeleteProjectDb.Command, CommandResponse>
{
    /// <summary>
    ///     DB Command for deleting a Project.
    /// </summary>
    /// <param name="ProjectId">Internal ID of the project</param>
    public record Command(Guid ProjectId) : IValidatableRequest<CommandResponse>
    {
        /// <inheritdoc />
        public (bool valid, string? invalidReason) IsRequestValid()
        {
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
        Project? project = await dbContext.Projects.Include(project => project.Initiatives).FirstOrDefaultAsync(p => p.Id == request.ProjectId, cancellationToken);
        if (project == null)
        {
            return new()
            {
                Success = false,
                FailureReason = "Invalid ProjectId."
            };
        }

        foreach (Initiative initiative in project.Initiatives)
        {
            await sender.Send(new DeleteInitiativeDb.Command(initiative.Id), cancellationToken);
        }

        project.DeleteFlag = true;

        await dbContext.SaveChangesAsync(cancellationToken);

        return new()
        {
            Success = true
        };
    }
}