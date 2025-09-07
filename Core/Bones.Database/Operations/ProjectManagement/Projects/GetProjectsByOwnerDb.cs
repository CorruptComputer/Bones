using Bones.Database.DbSets.ProjectManagement;
using Bones.Shared.Backend.Enums;

namespace Bones.Database.Operations.ProjectManagement.Projects;

/// <inheritdoc />
public sealed class GetProjectsByOwnerDb(BonesDbContext dbContext) : IRequestHandler<GetProjectsByOwnerDb.Query, QueryResponse<List<Project>>>
{
    /// <summary>
    ///   DB Query to get projects by its owner
    /// </summary>
    /// <param name="OwnerType">The owner type</param>
    /// <param name="OwnerId">The owner id</param>
    public sealed record Query(OwnershipType OwnerType, Guid OwnerId) : IRequest<QueryResponse<List<Project>>>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Query>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.OwnerType).NotNull().IsInEnum();
            RuleFor(x => x.OwnerId).NotNull().NotEqual(Guid.Empty);
        }
    }

    /// <inheritdoc />
    public async Task<QueryResponse<List<Project>>> Handle(Query request, CancellationToken cancellationToken)
    {
        if (request.OwnerType == OwnershipType.User)
        {
            List<Project> projects = await dbContext.Projects.Where(p => p.OwningUser != null && p.OwningUser.Id == request.OwnerId).ToListAsync(cancellationToken);
            return QueryResponse<List<Project>>.Pass(projects);
        }

        return QueryResponse<List<Project>>.Fail("org not implemented");
    }
}