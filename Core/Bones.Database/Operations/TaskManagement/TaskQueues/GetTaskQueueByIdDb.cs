using Bones.Database.DbSets.TaskManagement;

namespace Bones.Database.Operations.TaskManagement.TaskQueues;

/// <inheritdoc />
public class GetTaskQueueByIdDb(BonesDbContext dbContext) : IRequestHandler<GetTaskQueueByIdDb.Query, QueryResponse<TaskQueue?>>
{
    /// <summary>
    ///   DB query for getting a task queue by its ID.
    /// </summary>
    /// <param name="TaskQueueId">Internal ID of the task queue</param>
    public record Query(Guid TaskQueueId) : IRequest<QueryResponse<TaskQueue?>>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Query>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.TaskQueueId).NotNull().NotEqual(Guid.Empty);
        }
    }

    /// <inheritdoc />
    public async Task<QueryResponse<TaskQueue?>> Handle(Query request, CancellationToken cancellationToken)
    {
        return await dbContext.TaskQueues
            .Include(q => q.Initiative)
                .ThenInclude(i => i.Project)
            .Include(q => q.Tasks)
                .ThenInclude(wi => wi.Item)
                .ThenInclude(i => i.Versions)
            .FirstOrDefaultAsync(x => x.Id == request.TaskQueueId, cancellationToken);
    }
}
