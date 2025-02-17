using Bones.Database.DbSets.WorkItemManagement;

namespace Bones.Database.Operations.WorkItemManagement.WorkItemQueues;

/// <inheritdoc />
public sealed class GetWorkItemQueuesByInitiativeDb(BonesDbContext dbContext) : IRequestHandler<GetWorkItemQueuesByInitiativeDb.Query, QueryResponse<List<WorkItemQueue>>>
{
    /// <summary>
    ///   Backend query for getting work item queues that belong to an initiative.
    /// </summary>
    /// <param name="InitiativeId">Internal ID of the initiative</param>
    public record Query(Guid InitiativeId) : IRequest<QueryResponse<List<WorkItemQueue>>>;

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
    public async Task<QueryResponse<List<WorkItemQueue>>> Handle(Query request, CancellationToken cancellationToken)
    {
        return (await dbContext.Initiatives.Include(i => i.Queues).FirstOrDefaultAsync(i => i.Id == request.InitiativeId, cancellationToken))?.Queues
            // Should only really happen if the initiative doesn't exist, but that would have been checked in the Logic layer before here anyways
            ?? [];
    }
}
