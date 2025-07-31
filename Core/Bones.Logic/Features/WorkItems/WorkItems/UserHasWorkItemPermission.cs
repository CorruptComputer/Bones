using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.WorkItemManagement;
using Bones.Database.Operations.WorkItemManagement.WorkItems;
using Bones.Logic.Features.WorkItems.Queue;

namespace Bones.Logic.Features.WorkItems.WorkItems;

/// <inheritdoc />
public class UserHasWorkItemPermission(ISender sender)
    : IRequestHandler<UserHasWorkItemPermission.Query, QueryResponse<bool>>
{
    /// <summary>
    ///   Checks if the user has permission to do the specified action in the initiative.
    /// </summary>
    /// <param name="WorkItemId"></param>
    /// <param name="User"></param>
    /// <param name="Claim"></param>
    public sealed record Query(Guid WorkItemId, BonesUser User, string Claim) : IRequest<QueryResponse<bool>>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Query>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.WorkItemId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.User).NotNull();
            RuleFor(x => x.Claim).NotNull().NotEmpty().Custom((claim, ctx) =>
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
        WorkItem? workItem = await sender.Send(new GetWorkItemByIdDb.Query(request.WorkItemId), cancellationToken);
        if (workItem is null)
        {
            return QueryResponse<bool>.Fail("Work Item not found");
        }

        bool? queuePermission = await sender.Send(
            new UserHasWorkItemQueuePermission.Query(workItem.WorkItemQueue.Id, request.User, request.Claim),
            cancellationToken);

        if (queuePermission == true)
        {
            return true;
        }

        return false;
    }
}
