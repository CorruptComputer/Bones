using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.Items;
using Bones.Database.DbSets.TaskManagement;
using Bones.Database.Operations.TaskManagement.TaskQueues;
using Bones.Database.Operations.TaskManagement.Tasks;
using Bones.Logic.Features.Tasks.TaskQueues;
using Bones.Shared.Consts;

namespace Bones.Logic.Features.Tasks.Tasks;

/// <inheritdoc />
public sealed class CreateTaskInQueue(ISender sender) : IRequestHandler<CreateTaskInQueue.Command, CommandResponse>
{
    /// <summary>
    ///   Command for creating a Task in a Queue.
    /// </summary>
    /// <param name="QueueId">Internal ID of the queue</param>
    /// <param name="TaskLayoutId"></param>
    /// <param name="TaskLayoutVersionId"></param>
    /// <param name="Title">The title to use for this item</param>
    /// <param name="Values"></param>
    /// <param name="ActionDateTime"></param>
    /// <param name="RequestingUser"></param>
    public sealed record Command(Guid QueueId, Guid TaskLayoutId, Guid TaskLayoutVersionId, string Title, Dictionary<Guid, object?> Values, DateTimeOffset ActionDateTime, BonesUser RequestingUser) : IRequest<CommandResponse>;
    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.QueueId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.TaskLayoutId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.TaskLayoutVersionId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.Title).NotEmpty().MaximumLength(256);
            RuleFor(x => x.Values).NotNull().ChildRules(dict =>
            {
                dict.RuleForEach(x => x.Keys).NotEmpty();
            });
            RuleFor(x => x.ActionDateTime).NotNull().LessThanOrEqualTo(DateTimeOffset.UtcNow)
                .WithMessage("Action date time cannot be in the future");
            RuleFor(x => x.RequestingUser).NotNull();
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        TaskQueue? queue = await sender.Send(new GetTaskQueueByIdDb.Query(request.QueueId), cancellationToken);
        if (queue is null)
        {
            return CommandResponse.Fail("Queue not found");
        }

        bool? permission = await sender.Send(new UserHasTaskQueuePermission.Query(queue.Id, request.RequestingUser, BonesClaimTypes.Role.Task.CREATE_TASK), cancellationToken);
        if (permission != true)
        {
            return CommandResponse.Forbid();
        }

        CommandResponse task = await sender.Send(new CreateTaskDb.Command(request.QueueId, request.TaskLayoutId, request.ActionDateTime), cancellationToken);

        if (!task.Success)
        {
            return task;
        }

        CommandResponse itemVersion = await sender.Send(new CreateTaskVersionDb.Command(task.Ids[nameof(BonesTask)], request.Title, request.TaskLayoutVersionId, request.Values, request.ActionDateTime), cancellationToken);

        if (!itemVersion.Success)
        {
            return itemVersion;
        }

        Dictionary<string, Guid> ids = new()
        {
            { nameof(BonesTask), task.Ids[nameof(BonesTask)] },
            { nameof(ItemVersion), itemVersion.Ids[nameof(ItemVersion)] }
        };

        return CommandResponse.Pass(ids);
    }
}