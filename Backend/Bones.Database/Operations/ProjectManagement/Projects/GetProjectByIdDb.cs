using Bones.Database.DbSets.ProjectManagement;

namespace Bones.Database.Operations.ProjectManagement.Projects;

/// <summary>
///   DB Query to get the project by its ID.
/// </summary>
/// <param name="ProjectId">The Project ID</param>
public sealed record GetProjectByIdDbQuery(Guid ProjectId) : IRequest<QueryResponse<Project>>;

internal sealed class GetProjectByIdDbQueryValidator : AbstractValidator<GetProjectByIdDbQuery>
{
    public override Task<ValidationResult> ValidateAsync(ValidationContext<GetProjectByIdDbQuery> context, CancellationToken cancellation = new())
    {
        RuleFor(x => x.ProjectId).NotNull().NotEqual(Guid.Empty);

        return base.ValidateAsync(context, cancellation);
    }
}

internal sealed class GetProjectByIdDb(BonesDbContext dbContext) : IRequestHandler<GetProjectByIdDbQuery, QueryResponse<Project>>
{
    public async Task<QueryResponse<Project>> Handle(GetProjectByIdDbQuery request, CancellationToken cancellationToken)
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