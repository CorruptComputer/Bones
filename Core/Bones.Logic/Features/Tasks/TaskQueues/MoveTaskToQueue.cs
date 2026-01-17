using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.TaskManagement;
using Bones.Database.Operations.TaskManagement.TaskQueues;
using Bones.Database.Operations.TaskManagement.Tasks;
using Bones.Shared.Consts;

namespace Bones.Logic.Features.Tasks.TaskQueues;

/// <inheritdoc />
public sealed class MoveTaskToQueue(ISender sender) : IRequestHandler<MoveTaskToQueue.Command, CommandResponse>
{
    /// <summary>
    ///   Command for moving a Task to a Queue.
    /// </summary>
    /// <param name="TaskId"></param>
    /// <param name="QueueId">Internal ID of the queue</param>
    /// <param name="ActionDateTime"></param>
    /// <param name="RequestingUser"></param>
    public sealed record Command(Guid TaskId, Guid QueueId, DateTimeOffset ActionDateTime, BonesUser RequestingUser) : IRequest<CommandResponse>;
    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.TaskId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.QueueId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.ActionDateTime).NotNull().LessThanOrEqualTo(DateTimeOffset.UtcNow)
                .WithMessage("Action date time cannot be in the future");
            RuleFor(x => x.RequestingUser).NotNull();
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        BonesTask? task = await sender.Send(new GetTaskByIdDb.Query(request.TaskId), cancellationToken);
        if (task is null)
        {
            return CommandResponse.Forbid();
        }

        bool? currentQueuePermission = await sender.Send(new UserHasTaskQueuePermission.Query(task.TaskQueueId, request.RequestingUser, BonesClaimTypes.Role.Task.EDIT_TASK), cancellationToken);
        if (currentQueuePermission != true)
        {
            return CommandResponse.Forbid();
        }

        TaskQueue? currentQueue = await sender.Send(new GetTaskQueueByIdDb.Query(task.TaskQueueId), cancellationToken);
        if (currentQueue is null)
        {
            return CommandResponse.Fail("Queue not found");
        }

        bool? newQueuePermission = await sender.Send(new UserHasTaskQueuePermission.Query(request.QueueId, request.RequestingUser, BonesClaimTypes.Role.Task.EDIT_TASK), cancellationToken);
        if (newQueuePermission != true)
        {
            return CommandResponse.Forbid();
        }

        TaskQueue? newQueue = await sender.Send(new GetTaskQueueByIdDb.Query(request.QueueId), cancellationToken);
        if (newQueue is null)
        {
            return CommandResponse.Fail("Queue not found");
        }

        if (currentQueue.Id == newQueue.Id)
        {
            return CommandResponse.Fail("Task is already in the specified queue");
        }

        if (currentQueue.Initiative!.ProjectId != newQueue.Initiative!.ProjectId)
        {
            return CommandResponse.Fail("Cannot move task to a queue in a different project");
        }

        return await sender.Send(new MoveTaskToQueueDb.Command(request.TaskId, request.QueueId), cancellationToken);
    }
}