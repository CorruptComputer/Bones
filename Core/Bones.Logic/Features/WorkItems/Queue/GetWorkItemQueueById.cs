using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.WorkItemManagement;
using Bones.Database.Operations.WorkItemManagement.WorkItemQueues;
using Bones.Shared.Consts;

namespace Bones.Logic.Features.WorkItems.Queue;

/// <inheritdoc />
public class GetWorkItemQueueById(ISender sender) : IRequestHandler<GetWorkItemQueueById.Query, QueryResponse<WorkItemQueue?>>
{
    /// <summary>
    ///   Backend query for getting work item queues that belong to an initiative.
    /// </summary>
    /// <param name="WorkItemQueueId">Internal ID of the work item queue</param>
    /// <param name="RequestingUser">The user requesting this</param>
    public record Query(Guid WorkItemQueueId, BonesUser RequestingUser) : IRequest<QueryResponse<WorkItemQueue?>>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Query>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.WorkItemQueueId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.RequestingUser).NotNull();
        }
    }

    /// <inheritdoc />
    public async Task<QueryResponse<WorkItemQueue?>> Handle(Query request, CancellationToken cancellationToken)
    {
        const string perm = BonesClaimTypes.Role.WorkItemQueue.VIEW_QUEUE;
        bool? hasQueuePermission =
            await sender.Send(new UserHasWorkItemQueuePermission.Query(request.WorkItemQueueId, request.RequestingUser, perm), cancellationToken);

        if (hasQueuePermission != true)
        {
            return QueryResponse<WorkItemQueue?>.Forbid();
        }

        return await sender.Send(new GetWorkItemQueueByIdDb.Query(request.WorkItemQueueId), cancellationToken);
    }
}
