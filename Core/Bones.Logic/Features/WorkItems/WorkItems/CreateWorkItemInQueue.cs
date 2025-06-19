using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.WorkItemManagement;
using Bones.Database.Operations.WorkItemManagement.WorkItemQueues;
using Bones.Database.Operations.WorkItemManagement.WorkItems;
using Bones.Logic.Features.WorkItems.Queue;
using Bones.Shared.Consts;

namespace Bones.Logic.Features.WorkItems.WorkItems;

/// <inheritdoc />
public sealed class CreateWorkItemInQueue(ISender sender) : IRequestHandler<CreateWorkItemInQueue.Command, CommandResponse>
{
    /// <summary>
    ///   Command for creating a Work Item in a Queue.
    /// </summary>
    /// <param name="QueueId">Internal ID of the queue</param>
    /// <param name="WorkItemLayoutId"></param>
    /// <param name="WorkItemLayoutVersionId"></param>
    /// <param name="Title">The title to use for this item</param>
    /// <param name="Values"></param>
    /// <param name="RequestingUser"></param>
    public sealed record Command(Guid QueueId, Guid WorkItemLayoutId, Guid WorkItemLayoutVersionId, string Title, Dictionary<Guid, object?> Values, BonesUser RequestingUser) : IRequest<CommandResponse>;
    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.QueueId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.WorkItemLayoutId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.WorkItemLayoutVersionId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.Title).NotNull().NotEmpty().MaximumLength(256);
            RuleFor(x => x.Values).NotNull().ChildRules(dict =>
            {
                dict.RuleForEach(x => x.Keys).NotNull().NotEmpty();
            });
            RuleFor(x => x.RequestingUser).NotNull();
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        WorkItemQueue? queue = await sender.Send(new GetWorkItemQueueByIdDb.Query(request.QueueId), cancellationToken);
        if (queue is null)
        {
            return CommandResponse.Fail("Queue not found");
        }

        bool? permission = await sender.Send(new UserHasWorkItemQueuePermission.Query(queue.Id, request.RequestingUser, BonesClaimTypes.Role.WorkItem.CREATE_WORK_ITEM), cancellationToken);
        if (permission != true)
        {
            return CommandResponse.Forbid();
        }

        CommandResponse item = await sender.Send(new CreateWorkItemDb.Command(request.QueueId, request.WorkItemLayoutId), cancellationToken);

        if (item.Success && item.Id is not null)
        {
            return await sender.Send(new CreateWorkItemVersionDb.Command(item.Id.Value, request.Title, request.WorkItemLayoutVersionId, request.Values), cancellationToken);
        }

        return item;
    }
}