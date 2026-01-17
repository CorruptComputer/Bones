using Bones.Database.DbSets.ProjectManagement;

namespace Bones.Database.Operations.ProjectManagement.Initiatives;

/// <inheritdoc />
public sealed class GetInitiativesByProjectDb(BonesDbContext dbContext) : IRequestHandler<GetInitiativesByProjectDb.Query, QueryResponse<List<Initiative>>>
{
    /// <summary>
    ///   DB Query for getting the initiatives under a project
    /// </summary>
    /// <param name="ProjectId">Internal ID of the project</param>
    public record Query(Guid ProjectId) : IRequest<QueryResponse<List<Initiative>>>;

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
    public async Task<QueryResponse<List<Initiative>>> Handle(Query request, CancellationToken cancellationToken)
    {
        return await dbContext.Initiatives
            .Include(i => i.Queues)
            .Where(i => i.Project!.Id == request.ProjectId)
            .ToListAsync(cancellationToken);
    }
}