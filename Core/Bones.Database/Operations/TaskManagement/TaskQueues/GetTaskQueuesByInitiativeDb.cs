using Bones.Database.DbSets.TaskManagement;

namespace Bones.Database.Operations.TaskManagement.TaskQueues;

/// <inheritdoc />
public sealed class GetTaskQueuesByInitiativeDb(BonesDbContext dbContext) : IRequestHandler<GetTaskQueuesByInitiativeDb.Query, QueryResponse<List<TaskQueue>>>
{
    /// <summary>
    ///   Backend query for getting task queues that belong to an initiative.
    /// </summary>
    /// <param name="InitiativeId">Internal ID of the initiative</param>
    public record Query(Guid InitiativeId) : IRequest<QueryResponse<List<TaskQueue>>>;

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
    public async Task<QueryResponse<List<TaskQueue>>> Handle(Query request, CancellationToken cancellationToken)
    {
        return (await dbContext.Initiatives.Include(i => i.Queues).FirstOrDefaultAsync(i => i.Id == request.InitiativeId, cancellationToken))?.Queues
            // Should only really happen if the initiative doesn't exist, but that would have been checked in the Logic layer before here anyways
            ?? [];
    }
}
