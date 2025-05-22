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
    ///     Command for creating a Queue.
    /// </summary>
    /// <param name="Name">Name of the queue</param>
    /// <param name="QueueId">Internal ID of the queue</param>
    /// <param name="WorkItemLayoutId"></param>
    /// <param name="WorkItemLayoutVersionId"></param>
    /// <param name="Values"></param>
    /// <param name="RequestingUser"></param>
    public sealed record Command(string Name, Guid QueueId, Guid WorkItemLayoutId, Guid WorkItemLayoutVersionId, Dictionary<Guid, object?> Values, BonesUser RequestingUser) : IRequest<CommandResponse>;

    // Item
    // public sealed record Command(string Name, Guid QueueId, Guid ItemLayoutId) : IRequest<CommandResponse>;

    // Version
    // public record Command(Guid WorkItemId, Guid WorkItemLayoutVersionId, Dictionary<Guid, object?> Values) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.Name).NotNull().NotEmpty();
            RuleFor(x => x.QueueId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.WorkItemLayoutVersionId).NotNull().NotEqual(Guid.Empty);
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

        CommandResponse item = await sender.Send(new CreateWorkItemDb.Command(request.Name, request.QueueId, request.WorkItemLayoutId), cancellationToken);

        if (item.Success && item.Id is not null)
        {
            return await sender.Send(new CreateWorkItemVersionDb.Command(item.Id.Value, request.WorkItemLayoutVersionId, request.Values), cancellationToken);
        }

        return item;
    }
}