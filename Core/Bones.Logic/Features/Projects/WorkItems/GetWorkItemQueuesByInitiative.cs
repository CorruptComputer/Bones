using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.WorkItemManagement;
using Bones.Database.Operations.WorkItemManagement.WorkItemQueues;
using Bones.Logic.Features.Projects.Initiatives;
using Bones.Shared.Consts;

namespace Bones.Logic.Features.Projects.WorkItems;

/// <summary>
///   Backend query for getting work item queues that belong to an initiative.
/// </summary>
/// <param name="InitiativeId">Internal ID of the initiative</param>
/// <param name="RequestingUser">The user requesting this</param>
public record GetWorkItemQueuesByInitiativeQuery(Guid InitiativeId, BonesUser RequestingUser) : IRequest<QueryResponse<List<WorkItemQueue>>>;

internal sealed class GetWorkItemQueuesByInitiativeQueryValidator : AbstractValidator<GetWorkItemQueuesByInitiativeQuery>
{

}

internal sealed class GetWorkItemQueuesByInitiativeHandler(ISender sender) : IRequestHandler<GetWorkItemQueuesByInitiativeQuery, QueryResponse<List<WorkItemQueue>>>
{
    public async Task<QueryResponse<List<WorkItemQueue>>> Handle(GetWorkItemQueuesByInitiativeQuery request, CancellationToken cancellationToken)
    {
        const string perm = BonesClaimTypes.Role.WorkItemQueue.VIEW_QUEUE;
        bool? hasOrganizationPermission =
            await sender.Send(new UserHasInitiativePermissionQuery(request.InitiativeId, request.RequestingUser, perm), cancellationToken);

        if (hasOrganizationPermission != true)
        {
            return QueryResponse<List<WorkItemQueue>>.Forbid();
        }

        return await sender.Send(new GetWorkItemQueuesByInitiativeDbQuery(request.InitiativeId), cancellationToken);
    }
}
