using Bones.Database.DbSets.ProjectManagement;

namespace Bones.Database.Operations.ProjectManagement.Projects;

/// <inheritdoc />
public sealed class GetProjectByIdDb(BonesDbContext dbContext) : IRequestHandler<GetProjectByIdDb.Query, QueryResponse<Project>>
{
    /// <summary>
    ///   DB Query to get the project by its ID.
    /// </summary>
    /// <param name="ProjectId">The Project ID</param>
    public sealed record Query(Guid ProjectId) : IRequest<QueryResponse<Project>>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Query>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.ProjectId).NotNull().NotEqual(Guid.Empty);
        }
    }

    /// <inheritdoc />
    public async Task<QueryResponse<Project>> Handle(Query request, CancellationToken cancellationToken)
    {
        Project? project = await dbContext.Projects
            .Include(p => p.OwningOrganization)
            .Include(p => p.OwningUser)
            .FirstOrDefaultAsync(x => x.Id == request.ProjectId, cancellationToken);

        if (project is null)
        {
            return QueryResponse<Project>.Fail("Project not found");
        }

        return QueryResponse<Project>.Pass(project);
    }
}