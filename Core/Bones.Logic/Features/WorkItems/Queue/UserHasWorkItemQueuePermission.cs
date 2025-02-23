using System.Security.Claims;
using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.WorkItemManagement;
using Bones.Database.Operations.WorkItemManagement.WorkItemQueues;
using Bones.Logic.Features.Projects.Initiatives;
using Bones.Shared.Consts;
using Microsoft.AspNetCore.Identity;

namespace Bones.Logic.Features.WorkItems.Queue;

/// <inheritdoc />
public class UserHasWorkItemQueuePermission(UserManager<BonesUser> userManager, RoleManager<BonesRole> roleManager, ISender sender)
    : IRequestHandler<UserHasWorkItemQueuePermission.Query, QueryResponse<bool>>
{
    /// <summary>
    ///   Checks if the user has permission to do the specified action in the initiative.
    /// </summary>
    /// <param name="WorkItemQueueId"></param>
    /// <param name="User"></param>
    /// <param name="Claim"></param>
    public sealed record Query(Guid WorkItemQueueId, BonesUser User, string Claim) : IRequest<QueryResponse<bool>>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Query>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.WorkItemQueueId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.User).NotNull();
            RuleFor(x => x.Claim).NotNull().NotEmpty().Custom((claim, ctx) =>
            {
                if (claim.Contains('|'))
                {
                    ctx.AddFailure("Claim contains '|', this means you probably called GetInitiativeClaimType(). Don't do that, just pass in the claim name.");
                }
            });
        }
    }

    /// <inheritdoc />
    public async Task<QueryResponse<bool>> Handle(Query request, CancellationToken cancellationToken)
    {
        WorkItemQueue? workItemQueue = await sender.Send(new GetWorkItemQueueByIdDb.Query(request.WorkItemQueueId), cancellationToken);

        if (workItemQueue is null)
        {
            return QueryResponse<bool>.Fail("Work Item Queue not found");
        }

        bool? initiativePermission = await sender.Send(
            new UserHasInitiativePermission.Query(workItemQueue.Initiative.Id, request.User, request.Claim),
            cancellationToken);

        if (initiativePermission == true)
        {
            return true;
        }

        foreach (string roleName in await userManager.GetRolesAsync(request.User))
        {
            BonesRole? role = await roleManager.FindByNameAsync(roleName);
            if (role is null)
            {
                return QueryResponse<bool>.Fail("Role not found");
            }

            string neededClaim = BonesClaimTypes.Role.WorkItemQueue.GetWorkItemQueueClaimType(workItemQueue.Id, request.Claim);

            IList<Claim> claims = await roleManager.GetClaimsAsync(role);
            if (claims.Any(claim => claim.Type == neededClaim && claim.Value == ClaimValues.YES))
            {
                return true;
            }
        }

        return false;
    }
}
