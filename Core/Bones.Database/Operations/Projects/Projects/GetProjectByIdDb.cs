using Bones.Database.DbSets.Projects;

namespace Bones.Database.Operations.Projects.Projects;

/// <inheritdoc />
public sealed class GetProjectByIdDb(BonesDbContext dbContext) : IRequestHandler<GetProjectByIdDb.Query, QueryResponse<Project>>
{
    /// <summary>
    ///   DB Query to get the project by its ID.
    /// </summary>
    /// <param name="ProjectId">The Project ID</param>
    /// <param name="IncludeOwner">Whether to include the owning user and organization</param>
    public sealed record Query(Guid ProjectId, bool IncludeOwner = false) : IRequest<QueryResponse<Project>>;

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
        IQueryable<Project> projectQuery = dbContext.Projects;
        if (request.IncludeOwner)
        {
            projectQuery = projectQuery.Include(p => p.OwningOrganization)
                                       .Include(p => p.OwningUser);
        }

        Project? project = await projectQuery.FirstOrDefaultAsync(x => x.Id == request.ProjectId, cancellationToken);
        if (project is null)
        {
            return QueryResponse<Project>.Fail("Project not found");
        }

        return QueryResponse<Project>.Pass(project);
    }
}