using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.Items;
using Bones.Database.DbSets.TaskManagement;
using Bones.Database.Operations.AccountManagement;
using Bones.Database.Operations.Items;
using Bones.Database.Operations.TaskManagement.Tasks;
using Bones.Shared.Backend.Enums;
using Bones.Shared.Consts;

namespace Bones.Logic.Features.Tasks.Tasks;

/// <inheritdoc />
public sealed class AssignTask(ISender sender) : IRequestHandler<AssignTask.Command, CommandResponse>
{
    /// <summary>
    ///   Command for assigning a task to a user
    /// </summary>
    /// <param name="TaskId">Internal ID of the task</param>
    /// <param name="AssignmentSlotId">ID of the assignment slot</param>
    /// <param name="BonesUserId">ID of the user to assign to</param>
    /// <param name="State">State for the assignment</param>
    /// <param name="RequestingUser">The user making the request</param>
    public record Command(Guid TaskId, Guid AssignmentSlotId, Guid BonesUserId, string State, BonesUser RequestingUser) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.TaskId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.AssignmentSlotId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.BonesUserId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.State).NotEmpty();
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        const string perm = BonesClaimTypes.Role.Task.EDIT_TASK;
        bool? hasTaskPermission =
            await sender.Send(new UserHasTaskPermission.Query(request.TaskId, request.RequestingUser, perm), cancellationToken);

        if (hasTaskPermission != true)
        {
            return CommandResponse.Forbid();
        }

        // Get the task
        BonesTask? task = await sender.Send(new GetTaskByIdDb.Query(request.TaskId), cancellationToken);

        if (task is null)
        {
            return CommandResponse.Fail("Task not found");
        }

        // Get the assignee slot
        ItemAssignmentSlot? assigneeSlot = task.Item!.Current!.ItemLayoutVersion?.ItemAssignmentSlots.FirstOrDefault(x => x.Id == request.AssignmentSlotId);
        if (assigneeSlot is null)
        {
            return CommandResponse.Fail("Assignment slot not found");
        }

        // Check if the assignment slot is full
        if (assigneeSlot.SelectionType == SelectionType.Single)
        {
            bool isAlreadyAssigned = task.Item.Current!.ItemAssignees.Any(x => x.ItemAssignmentSlotId == request.AssignmentSlotId);
            if (isAlreadyAssigned)
            {
                return CommandResponse.Fail("Assignment slot is already filled");
            }
        }

        // Check if the assignment slot accepts users
        if (assigneeSlot.AssignmentType != AssignmentType.User)
        {
            return CommandResponse.Fail("Assignment slot does not accept user assignees");
        }

        BonesUser? assigneeUser = await sender.Send(new GetUserByIdDb.Query(request.BonesUserId), cancellationToken);
        if (assigneeUser is null)
        {
            return CommandResponse.Fail("User not found");
        }

        // Check if the user being assigned has view permissions
        bool? hasViewPermission =
            await sender.Send(new UserHasTaskPermission.Query(request.TaskId, assigneeUser, BonesClaimTypes.Role.Task.VIEW_TASK), cancellationToken);

        if (hasViewPermission != true)
        {
            return CommandResponse.Fail("User does not have permission to view this task");
        }

        CommandResponse assignTaskResponse = await sender.Send(
            new AssignItemDb.Command(task.Item.Id, request.AssignmentSlotId, request.BonesUserId, request.State), cancellationToken);

        return assignTaskResponse;
    }
}