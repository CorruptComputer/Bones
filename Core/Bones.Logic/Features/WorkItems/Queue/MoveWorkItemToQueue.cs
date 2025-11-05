using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.Items;
using Bones.Database.DbSets.WorkItemManagement;
using Bones.Database.Operations.WorkItemManagement.WorkItemQueues;
using Bones.Database.Operations.WorkItemManagement.WorkItems;
using Bones.Logic.Features.WorkItems.Queue;
using Bones.Shared.Consts;

namespace Bones.Logic.Features.WorkItems.Queue;

/// <inheritdoc />
public sealed class MoveWorkItemToQueue(ISender sender) : IRequestHandler<MoveWorkItemToQueue.Command, CommandResponse>
{
    /// <summary>
    ///   Command for moving a Work Item to a Queue.
    /// </summary>
    /// <param name="WorkItemId"></param>
    /// <param name="QueueId">Internal ID of the queue</param>
    /// <param name="ActionDateTime"></param>
    /// <param name="RequestingUser"></param>
    public sealed record Command(Guid WorkItemId, Guid QueueId, DateTimeOffset ActionDateTime, BonesUser RequestingUser) : IRequest<CommandResponse>;
    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.WorkItemId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.QueueId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.ActionDateTime).NotNull().LessThanOrEqualTo(DateTimeOffset.UtcNow)
                .WithMessage("Action date time cannot be in the future");
            RuleFor(x => x.RequestingUser).NotNull();
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        WorkItem? workItem = await sender.Send(new GetWorkItemByIdDb.Query(request.WorkItemId), cancellationToken);
        if (workItem is null)
        {
            return CommandResponse.Forbid();
        }

        bool? currentQueuePermission = await sender.Send(new UserHasWorkItemQueuePermission.Query(workItem.WorkItemQueue.Id, request.RequestingUser, BonesClaimTypes.Role.WorkItem.EDIT_WORK_ITEM), cancellationToken);
        if (currentQueuePermission != true)
        {
            return CommandResponse.Forbid();
        }

        WorkItemQueue? currentQueue = await sender.Send(new GetWorkItemQueueByIdDb.Query(workItem.WorkItemQueue.Id), cancellationToken);
        if (currentQueue is null)
        {
            return CommandResponse.Fail("Queue not found");
        }

        bool? newQueuePermission = await sender.Send(new UserHasWorkItemQueuePermission.Query(request.QueueId, request.RequestingUser, BonesClaimTypes.Role.WorkItem.EDIT_WORK_ITEM), cancellationToken);
        if (newQueuePermission != true)
        {
            return CommandResponse.Forbid();
        }

        WorkItemQueue? newQueue = await sender.Send(new GetWorkItemQueueByIdDb.Query(request.QueueId), cancellationToken);
        if (newQueue is null)
        {
            return CommandResponse.Fail("Queue not found");
        }

        if (currentQueue.Id == newQueue.Id)
        {
            return CommandResponse.Fail("Work item is already in the specified queue");
        }

        if (currentQueue.Initiative.Project.Id != newQueue.Initiative.Project.Id)
        {
            return CommandResponse.Fail("Cannot move work item to a queue in a different project");
        }

        return await sender.Send(new MoveWorkItemToQueueDb.Command(request.WorkItemId, request.QueueId), cancellationToken);
    }
}