using Bones.Database.DbSets.Items.Types;

namespace Bones.Database.Operations.Items.Types.Tasks;

/// <inheritdoc />
public sealed class GetTaskCurrentAssigneesByIdDb(BonesDbContext dbContext) : IRequestHandler<GetTaskCurrentAssigneesByIdDb.Query, QueryResponse<BonesTask?>>
{
    /// <summary>
    ///   DB Command for getting a Tasks current assignees by the Task ID.
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
            .Include(t => t.Item).ThenInclude(i => i!.Versions).ThenInclude(iv => iv.ItemAssignees).ThenInclude(a => a.ItemAssignmentSlot)
            .Include(t => t.Item).ThenInclude(i => i!.Versions).ThenInclude(iv => iv.ItemAssignees).ThenInclude(a => a.AssignedUser)
            //.Include(t => t.Item).ThenInclude(i => i.Versions).ThenInclude(iv => iv.Assignees).ThenInclude(a => a.AssignedRole)
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == request.TaskId, cancellationToken);

        return task;
    }
}