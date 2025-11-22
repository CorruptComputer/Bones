using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.TaskManagement;
using Bones.Database.Operations.TaskManagement.TaskQueues;
using Bones.Shared.Consts;

namespace Bones.Logic.Features.Tasks.TaskQueues;

/// <inheritdoc />
public class GetTaskQueueById(ISender sender) : IRequestHandler<GetTaskQueueById.Query, QueryResponse<TaskQueue?>>
{
    /// <summary>
    ///   Backend query for getting task queues that belong to an initiative.
    /// </summary>
    /// <param name="TaskQueueId">Internal ID of the task queue</param>
    /// <param name="RequestingUser">The user requesting this</param>
    public record Query(Guid TaskQueueId, BonesUser RequestingUser) : IRequest<QueryResponse<TaskQueue?>>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Query>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.TaskQueueId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.RequestingUser).NotNull();
        }
    }

    /// <inheritdoc />
    public async Task<QueryResponse<TaskQueue?>> Handle(Query request, CancellationToken cancellationToken)
    {
        const string perm = BonesClaimTypes.Role.TaskQueue.VIEW_QUEUE;
        bool? hasQueuePermission =
            await sender.Send(new UserHasTaskQueuePermission.Query(request.TaskQueueId, request.RequestingUser, perm), cancellationToken);

        if (hasQueuePermission != true)
        {
            return QueryResponse<TaskQueue?>.Forbid();
        }

        return await sender.Send(new GetTaskQueueByIdDb.Query(request.TaskQueueId), cancellationToken);
    }
}
