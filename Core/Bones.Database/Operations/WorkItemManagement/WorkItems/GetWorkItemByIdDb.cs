using Bones.Database.DbSets.WorkItemManagement;

namespace Bones.Database.Operations.WorkItemManagement.WorkItems;

/// <inheritdoc />
public sealed class GetWorkItemByIdDb(BonesDbContext dbContext) : IRequestHandler<GetWorkItemByIdDb.Query, QueryResponse<WorkItem?>>
{
    /// <summary>
    ///   DB Command for getting a WorkItem by id.
    /// </summary>
    /// <param name="WorkItemId">ID of the item</param>
    public sealed record Query(Guid WorkItemId) : IRequest<QueryResponse<WorkItem?>>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Query>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.WorkItemId).NotNull().NotEqual(Guid.Empty);
        }
    }

    /// <inheritdoc />
    public async Task<QueryResponse<WorkItem?>> Handle(Query request, CancellationToken cancellationToken)
    {
        WorkItem? workItem = await dbContext.WorkItems
            .Include(i => i.Item).ThenInclude(i => i.Project)
            .Include(i => i.Item).ThenInclude(i => i.GenericItemLayout)
            .Include(i => i.Item).ThenInclude(i => i.Versions).ThenInclude(i => i.GenericItemLayoutVersion).ThenInclude(i => i.FieldLinks).ThenInclude(i => i.FieldVersion)
            .Include(i => i.Item).ThenInclude(i => i.Versions).ThenInclude(i => i.Values).ThenInclude(i => i.Field)
            .Include(i => i.WorkItemQueue)
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == request.WorkItemId, cancellationToken);

        return workItem;
    }
}