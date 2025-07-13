using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.WorkItemManagement;
using Bones.Database.Operations.WorkItemManagement.WorkItemQueues;
using Bones.Logic.Features.Initiatives;
using Bones.Shared.Consts;

namespace Bones.Logic.Features.WorkItems.Queue;

/// <inheritdoc />
public sealed class GetWorkItemQueuesByInitiative(ISender sender) : IRequestHandler<GetWorkItemQueuesByInitiative.Query, QueryResponse<List<WorkItemQueue>>>
{
    /// <summary>
    ///   Backend query for getting work item queues that belong to an initiative.
    /// </summary>
    /// <param name="InitiativeId">Internal ID of the initiative</param>
    /// <param name="RequestingUser">The user requesting this</param>
    public record Query(Guid InitiativeId, BonesUser RequestingUser) : IRequest<QueryResponse<List<WorkItemQueue>>>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Query>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.InitiativeId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.RequestingUser).NotNull();
        }
    }

    /// <inheritdoc />
    public async Task<QueryResponse<List<WorkItemQueue>>> Handle(Query request, CancellationToken cancellationToken)
    {
        const string perm = BonesClaimTypes.Role.WorkItemQueue.VIEW_QUEUE;
        bool? hasOrganizationPermission =
            await sender.Send(new UserHasInitiativePermission.Query(request.InitiativeId, request.RequestingUser, perm), cancellationToken);

        if (hasOrganizationPermission != true)
        {
            return QueryResponse<List<WorkItemQueue>>.Forbid();
        }

        return await sender.Send(new GetWorkItemQueuesByInitiativeDb.Query(request.InitiativeId), cancellationToken);
    }
}
