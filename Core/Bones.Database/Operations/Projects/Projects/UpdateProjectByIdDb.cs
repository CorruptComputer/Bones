using Bones.Database.DbSets.Projects;

namespace Bones.Database.Operations.Projects.Projects;

/// <inheritdoc />
public sealed class UpdateProjectByIdDb(BonesDbContext dbContext) : IRequestHandler<UpdateProjectByIdDb.Command, CommandResponse>
{
    /// <summary>
    ///   DB Command for updating a Project.
    /// </summary>
    /// <param name="ProjectId">Internal ID of the project</param>
    /// <param name="Name">The new name of the project</param>
    public record Command(Guid ProjectId, string Name) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.ProjectId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.Name).NotEmpty();
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        Project? project = await dbContext.Projects.FirstOrDefaultAsync(p => p.Id == request.ProjectId, cancellationToken);
        if (project == null)
        {
            return CommandResponse.Fail("Invalid ProjectId.");
        }

        project.Name = request.Name;
        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass();
    }
}