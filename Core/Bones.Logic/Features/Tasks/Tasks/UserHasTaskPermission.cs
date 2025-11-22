using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.TaskManagement;
using Bones.Database.Operations.TaskManagement.Tasks;
using Bones.Logic.Features.Tasks.TaskQueues;

namespace Bones.Logic.Features.Tasks.Tasks;

/// <inheritdoc />
public class UserHasTaskPermission(ISender sender)
    : IRequestHandler<UserHasTaskPermission.Query, QueryResponse<bool>>
{
    /// <summary>
    ///   Checks if the user has permission to do the specified action to the task.
    /// </summary>
    /// <param name="TaskId"></param>
    /// <param name="User"></param>
    /// <param name="Claim"></param>
    public sealed record Query(Guid TaskId, BonesUser User, string Claim) : IRequest<QueryResponse<bool>>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Query>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.TaskId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.User).NotNull();
            RuleFor(x => x.Claim).NotEmpty().Custom((claim, ctx) =>
            {
                if (claim.Contains('|'))
                {
                    ctx.AddFailure("Claim contains '|', this means you probably called Get*ClaimType(). Don't do that, just pass in the claim name.");
                }
            });
        }
    }

    /// <inheritdoc />
    public async Task<QueryResponse<bool>> Handle(Query request, CancellationToken cancellationToken)
    {
        BonesTask? task = await sender.Send(new GetTaskByIdDb.Query(request.TaskId), cancellationToken);
        if (task is null)
        {
            return QueryResponse<bool>.Fail("Task not found");
        }

        bool? queuePermission = await sender.Send(
            new UserHasTaskQueuePermission.Query(task.TaskQueue.Id, request.User, request.Claim),
            cancellationToken);

        if (queuePermission == true)
        {
            return true;
        }

        return false;
    }
}
