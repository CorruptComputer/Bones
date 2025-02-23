using Bones.Database.DbSets.WorkItemManagement;

namespace Bones.Database.Operations.WorkItemManagement.WorkItemQueues;

/// <inheritdoc />
public class GetWorkItemQueueByIdDb(BonesDbContext dbContext) : IRequestHandler<GetWorkItemQueueByIdDb.Query, QueryResponse<WorkItemQueue?>>
{
    /// <summary>
    ///   DB query for getting a work item queue by its ID.
    /// </summary>
    /// <param name="WorkItemQueueId">Internal ID of the work item queue</param>
    public record Query(Guid WorkItemQueueId) : IRequest<QueryResponse<WorkItemQueue?>>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Query>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.WorkItemQueueId).NotNull().NotEqual(Guid.Empty);
        }
    }

    /// <inheritdoc />
    public async Task<QueryResponse<WorkItemQueue?>> Handle(Query request, CancellationToken cancellationToken)
    {
        return await dbContext.WorkItemQueues
        .Include(q => q.Initiative)
        .Include(q => q.WorkItems)
        .ThenInclude(wi => wi.Item)
        .ThenInclude(i => i.Versions)
        .FirstOrDefaultAsync(x => x.Id == request.WorkItemQueueId, cancellationToken);
    }
}

