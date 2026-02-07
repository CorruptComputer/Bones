using Bones.Database.DbSets.Projects;

namespace Bones.Database.Operations.TaskManagement.TaskQueues;

/// <inheritdoc />
public class GetTaskQueueByIdDb(BonesDbContext dbContext) : IRequestHandler<GetTaskQueueByIdDb.Query, QueryResponse<TaskQueue?>>
{
    /// <summary>
    ///   DB query for getting a task queue by its ID.
    /// </summary>
    /// <param name="TaskQueueId">Internal ID of the task queue</param>
    /// <param name="IncludeInitiative">Whether to include the related initiative</param>
    /// <param name="IncludeTasks">Whether to include the related tasks</param>
    public record Query(Guid TaskQueueId, bool IncludeInitiative = false, bool IncludeTasks = false) : IRequest<QueryResponse<TaskQueue?>>;

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
        IQueryable<TaskQueue> query = dbContext.TaskQueues.AsNoTracking();

        if (request.IncludeInitiative)
        {
            query = query.Include(q => q.Initiative);
        }

        if (request.IncludeTasks)
        {
            query = query.Include(q => q.BonesTasks);
        }

        return await query.FirstOrDefaultAsync(x => x.Id == request.TaskQueueId, cancellationToken);
    }
}
