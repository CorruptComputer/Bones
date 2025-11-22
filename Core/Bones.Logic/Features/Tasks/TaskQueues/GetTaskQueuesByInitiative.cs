using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.TaskManagement;
using Bones.Database.Operations.TaskManagement.TaskQueues;
using Bones.Logic.Features.Initiatives;
using Bones.Shared.Consts;

namespace Bones.Logic.Features.Tasks.TaskQueues;

/// <inheritdoc />
public sealed class GetTaskQueuesByInitiative(ISender sender) : IRequestHandler<GetTaskQueuesByInitiative.Query, QueryResponse<List<TaskQueue>>>
{
    /// <summary>
    ///   Backend query for getting task queues that belong to an initiative.
    /// </summary>
    /// <param name="InitiativeId">Internal ID of the initiative</param>
    /// <param name="RequestingUser">The user requesting this</param>
    public record Query(Guid InitiativeId, BonesUser RequestingUser) : IRequest<QueryResponse<List<TaskQueue>>>;

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
    public async Task<QueryResponse<List<TaskQueue>>> Handle(Query request, CancellationToken cancellationToken)
    {
        const string perm = BonesClaimTypes.Role.TaskQueue.VIEW_QUEUE;
        bool? hasOrganizationPermission =
            await sender.Send(new UserHasInitiativePermission.Query(request.InitiativeId, request.RequestingUser, perm), cancellationToken);

        if (hasOrganizationPermission != true)
        {
            return QueryResponse<List<TaskQueue>>.Forbid();
        }

        return await sender.Send(new GetTaskQueuesByInitiativeDb.Query(request.InitiativeId), cancellationToken);
    }
}
