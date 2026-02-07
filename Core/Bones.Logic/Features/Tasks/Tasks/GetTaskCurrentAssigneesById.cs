using Bones.Database.DbSets.Accounts;
using Bones.Database.DbSets.Items.Assignments;
using Bones.Database.DbSets.Items.Types;
using Bones.Database.Operations.Items.Types.Tasks;
using Bones.Shared.Consts;

namespace Bones.Logic.Features.Tasks.Tasks;

/// <inheritdoc />
public sealed class GetTaskCurrentAssigneesById(ISender sender) : IRequestHandler<GetTaskCurrentAssigneesById.Query, QueryResponse<List<ItemAssignee>?>>
{
    /// <summary>
    ///   Retrieves the assignees for the current version of a task.
    /// </summary>
    /// <param name="TaskId">ID of the task</param>
    /// <param name="RequestingUser">The user requesting this data</param>
    public sealed record Query(Guid TaskId, BonesUser RequestingUser) : IRequest<QueryResponse<List<ItemAssignee>?>>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Query>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.TaskId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.RequestingUser).NotNull();
        }
    }

    /// <inheritdoc />
    public async Task<QueryResponse<List<ItemAssignee>?>> Handle(Query request, CancellationToken cancellationToken)
    {
        bool? permission = await sender.Send(new UserHasTaskPermission.Query(request.TaskId, request.RequestingUser, BonesClaimTypes.Role.Task.VIEW_TASK), cancellationToken);
        if (permission != true)
        {
            return QueryResponse<List<ItemAssignee>?>.Forbid();
        }

        BonesTask? task = await sender.Send(new GetTaskCurrentAssigneesByIdDb.Query(request.TaskId), cancellationToken);
        if (task is null)
        {
            return QueryResponse<List<ItemAssignee>?>.Fail("Task not found");
        }

        return task.Item?.Current?.ItemAssignees;
    }
}
