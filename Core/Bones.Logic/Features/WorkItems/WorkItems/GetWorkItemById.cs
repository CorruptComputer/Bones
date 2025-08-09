using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.WorkItemManagement;
using Bones.Database.Operations.WorkItemManagement.WorkItems;
using Bones.Shared.Consts;

namespace Bones.Logic.Features.WorkItems.WorkItems;

/// <inheritdoc />
public sealed class GetWorkItemById(ISender sender) : IRequestHandler<GetWorkItemById.Query, QueryResponse<WorkItem?>>
{
    /// <summary>
    ///   Command for creating a Queue.
    /// </summary>
    /// <param name="WorkItemId">Internal ID of the work item</param>
    /// <param name="RequestingUser"></param>
    public sealed record Query(Guid WorkItemId, BonesUser RequestingUser) : IRequest<QueryResponse<WorkItem?>>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Query>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.WorkItemId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.RequestingUser).NotNull();
        }
    }

    /// <inheritdoc />
    public async Task<QueryResponse<WorkItem?>> Handle(Query request, CancellationToken cancellationToken)
    {
        bool? permission = await sender.Send(new UserHasWorkItemPermission.Query(request.WorkItemId, request.RequestingUser, BonesClaimTypes.Role.WorkItem.VIEW_WORK_ITEM), cancellationToken);
        if (permission != true)
        {
            return QueryResponse<WorkItem?>.Forbid();
        }

        return await sender.Send(new GetWorkItemByIdDb.Query(request.WorkItemId), cancellationToken);
    }
}