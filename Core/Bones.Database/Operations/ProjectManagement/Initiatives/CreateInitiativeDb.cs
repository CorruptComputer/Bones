using Bones.Database.DbSets.ProjectManagement;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Bones.Database.Operations.ProjectManagement.Initiatives;

/// <inheritdoc />
public sealed class CreateInitiativeDb(BonesDbContext dbContext) : IRequestHandler<CreateInitiativeDb.Command, CommandResponse>
{
    /// <summary>
    ///     DB Command for creating an Initiative.
    /// </summary>
    /// <param name="Name">Name of the initiative</param>
    /// <param name="ProjectId">Internal ID of the project</param>
    public sealed record Command(string Name, Guid ProjectId) : IRequest<CommandResponse>;

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

        return CommandResponse.Pass(nameof(Initiative), created.Entity.Id);
    }
}