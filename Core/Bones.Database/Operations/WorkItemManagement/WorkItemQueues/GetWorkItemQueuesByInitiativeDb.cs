using Bones.Database.DbSets.WorkItemManagement;

namespace Bones.Database.Operations.WorkItemManagement.WorkItemQueues;

/// <summary>
///   Backend query for getting work item queues that belong to an initiative.
/// </summary>
/// <param name="InitiativeId">Internal ID of the initiative</param>
public record GetWorkItemQueuesByInitiativeDbQuery(Guid InitiativeId) : IRequest<QueryResponse<List<WorkItemQueue>>>;

internal sealed class GetWorkItemQueuesByInitiativeDbQueryValidator : AbstractValidator<GetWorkItemQueuesByInitiativeDbQuery>
{

}

internal sealed class GetWorkItemQueuesByInitiativeDbHandler(BonesDbContext dbContext) : IRequestHandler<GetWorkItemQueuesByInitiativeDbQuery, QueryResponse<List<WorkItemQueue>>>
{
    public async Task<QueryResponse<List<WorkItemQueue>>> Handle(GetWorkItemQueuesByInitiativeDbQuery request, CancellationToken cancellationToken)
    {
        return (await dbContext.Initiatives.Include(i => i.Queues).FirstOrDefaultAsync(i => i.Id == request.InitiativeId, cancellationToken))?.Queues
            // Should only really happen if the initiative doesn't exist, but that would have been checked in the Logic layer before here anyways
            ?? [];
    }
}
