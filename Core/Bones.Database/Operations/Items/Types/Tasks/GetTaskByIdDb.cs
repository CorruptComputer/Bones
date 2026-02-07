using Bones.Database.DbSets.Items.Types;

namespace Bones.Database.Operations.Items.Types.Tasks;

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
            .Include(t => t.Item).ThenInclude(i => i!.Project)
            .Include(t => t.Item).ThenInclude(i => i!.ItemLayout)
            .Include(t => t.Item).ThenInclude(i => i!.Versions).ThenInclude(iv => iv.ItemLayoutVersion).ThenInclude(ilv => ilv!.ItemLayoutFieldVersionLinks).ThenInclude(ilfvl => ilfvl.ItemFieldVersion).ThenInclude(ifv => ifv!.PossibleValues)
            .Include(t => t.Item).ThenInclude(i => i!.Versions).ThenInclude(iv => iv.ItemValues).ThenInclude(v => v.ItemFieldVersion)
            .Include(t => t.Item).ThenInclude(i => i!.Versions).ThenInclude(iv => iv.ItemAssignees)
            .Include(t => t.Item).ThenInclude(i => i!.Versions).ThenInclude(iv => iv.ItemLayoutVersion).ThenInclude(lv => lv!.ItemAssignmentSlots)
            .Include(t => t.TaskQueue)
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == request.TaskId, cancellationToken);

        return task;
    }
}