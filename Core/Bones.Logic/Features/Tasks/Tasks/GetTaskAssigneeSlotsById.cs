using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.Items;
using Bones.Database.DbSets.TaskManagement;
using Bones.Database.Operations.TaskManagement.Tasks;
using Bones.Shared.Consts;

namespace Bones.Logic.Features.Tasks.Tasks;

/// <inheritdoc />
public sealed class GetTaskAssigneeSlotsById(ISender sender) : IRequestHandler<GetTaskAssigneeSlotsById.Query, QueryResponse<List<ItemAssignmentSlot>?>>
{
    /// <summary>
    ///   Retrieves the assignee slots for the current version of a task.
    /// </summary>
    /// <param name="TaskId">ID of the task</param>
    /// <param name="RequestingUser">The user requesting this data</param>
    public sealed record Query(Guid TaskId, BonesUser RequestingUser) : IRequest<QueryResponse<List<ItemAssignmentSlot>?>>;

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
    public async Task<QueryResponse<List<ItemAssignmentSlot>?>> Handle(Query request, CancellationToken cancellationToken)
    {
        bool? permission = await sender.Send(new UserHasTaskPermission.Query(request.TaskId, request.RequestingUser, BonesClaimTypes.Role.Task.VIEW_TASK), cancellationToken);
        if (permission != true)
        {
            return QueryResponse<List<ItemAssignmentSlot>?>.Forbid();
        }

        BonesTask? task = await sender.Send(new GetTaskByIdDb.Query(request.TaskId), cancellationToken);
        if (task is null)
        {
            return QueryResponse<List<ItemAssignmentSlot>?>.Fail("Task not found");
        }

        return task.Item.Current?.ItemLayoutVersion.AssigneeSlots;
    }
}
