using Bones.Database.DbSets.TaskManagement;

namespace Bones.Database.Operations.TaskManagement.Tasks;

/// <inheritdoc />
public sealed class GetTaskByIdDb(BonesDbContext dbContext) : IRequestHandler<GetTaskByIdDb.Query, QueryResponse<BonesTask?>>
{
    /// <summary>
    ///   DB Command for getting a Task by id.
    /// </summary>
    /// <param name="TaskId">ID of the item</param>
    public sealed record Query(Guid TaskId) : IRequest<QueryResponse<BonesTask?>>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Query>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.TaskId).NotNull().NotEqual(Guid.Empty);
        }
    }

    /// <inheritdoc />
    public async Task<QueryResponse<BonesTask?>> Handle(Query request, CancellationToken cancellationToken)
    {
        BonesTask? task = await dbContext.Tasks
            .Include(wi => wi.Item).ThenInclude(i => i.Project)
            .Include(wi => wi.Item).ThenInclude(i => i.ItemLayout)
            .Include(wi => wi.Item).ThenInclude(i => i.Versions).ThenInclude(iv => iv.ItemLayoutVersion).ThenInclude(lv => lv.FieldLinks).ThenInclude(fl => fl.FieldVersion).ThenInclude(fv => fv.PossibleValues)
            .Include(wi => wi.Item).ThenInclude(i => i.Versions).ThenInclude(iv => iv.Values).ThenInclude(v => v.Field)
            .Include(wi => wi.Item).ThenInclude(i => i.Versions).ThenInclude(iv => iv.Assignees)
            .Include(wi => wi.Item).ThenInclude(i => i.Versions).ThenInclude(iv => iv.ItemLayoutVersion).ThenInclude(lv => lv.AssigneeSlots)
            .Include(i => i.TaskQueue)
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == request.TaskId, cancellationToken);

        return task;
    }
}