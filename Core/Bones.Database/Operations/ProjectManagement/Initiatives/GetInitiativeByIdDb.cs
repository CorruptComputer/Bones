using Bones.Database.DbSets.ProjectManagement;

namespace Bones.Database.Operations.ProjectManagement.Initiatives;

/// <inheritdoc />
public sealed class GetInitiativesByIdDb(BonesDbContext dbContext) : IRequestHandler<GetInitiativesByIdDb.Query, QueryResponse<Initiative>>
{
    /// <summary>
    ///   DB Query for getting an initiative
    /// </summary>
    /// <param name="InitiativeId">Internal ID of the initiative</param>
    public record Query(Guid InitiativeId) : IRequest<QueryResponse<Initiative>>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Query>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.InitiativeId).NotNull().NotEqual(Guid.Empty);
        }
    }

    /// <inheritdoc />
    public async Task<QueryResponse<Initiative>> Handle(Query request, CancellationToken cancellationToken)
    {
        return await dbContext.Initiatives
            .Include(i => i.Queues).ThenInclude(q => q.BonesTasks)
            .Include(i => i.Project)
            .FirstOrDefaultAsync(i => i.Id == request.InitiativeId, cancellationToken);
    }
}