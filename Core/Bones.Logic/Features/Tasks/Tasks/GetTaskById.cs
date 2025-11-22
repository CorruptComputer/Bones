using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.TaskManagement;
using Bones.Database.Operations.TaskManagement.Tasks;
using Bones.Shared.Consts;

namespace Bones.Logic.Features.Tasks.Tasks;

/// <inheritdoc />
public sealed class GetTaskById(ISender sender) : IRequestHandler<GetTaskById.Query, QueryResponse<BonesTask?>>
{
    /// <summary>
    ///   Command for creating a Queue.
    /// </summary>
    /// <param name="TaskId">Internal ID of the task</param>
    /// <param name="RequestingUser"></param>
    public sealed record Query(Guid TaskId, BonesUser RequestingUser) : IRequest<QueryResponse<BonesTask?>>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Query>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.TaskId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.RequestingUser).NotNull();
        }
    }

    /// <inheritdoc />
    public async Task<QueryResponse<BonesTask?>> Handle(Query request, CancellationToken cancellationToken)
    {
        bool? permission = await sender.Send(new UserHasTaskPermission.Query(request.TaskId, request.RequestingUser, BonesClaimTypes.Role.Task.VIEW_TASK), cancellationToken);
        if (permission != true)
        {
            return QueryResponse<BonesTask?>.Forbid();
        }

        return await sender.Send(new GetTaskByIdDb.Query(request.TaskId), cancellationToken);
    }
}