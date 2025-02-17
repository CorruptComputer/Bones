using Bones.Database.DbSets.ProjectManagement;
using Bones.Database.Operations.ProjectManagement.Initiatives;

namespace Bones.Database.Operations.ProjectManagement.Projects;

/// <inheritdoc />
public sealed class QueueDeleteProjectByIdDb(BonesDbContext dbContext, ISender sender) : IRequestHandler<QueueDeleteProjectByIdDb.Command, CommandResponse>
{
    /// <summary>
    ///     DB Command for deleting a Project.
    /// </summary>
    /// <param name="ProjectId">Internal ID of the project</param>
    public sealed record Command(Guid ProjectId) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.ProjectId).NotNull().NotEqual(Guid.Empty);
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        Project? project = await dbContext.Projects.Include(project => project.Initiatives).FirstOrDefaultAsync(p => p.Id == request.ProjectId, cancellationToken);
        if (project == null)
        {
            return CommandResponse.Fail("Invalid ProjectId.");
        }

        foreach (Initiative initiative in project.Initiatives)
        {
            await sender.Send(new QueueDeleteInitiativeByIdDb.Command(initiative.Id), cancellationToken);
        }

        project.DeleteFlag = true;

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass();
    }
}