using System.Security.Claims;
using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.TaskManagement;
using Bones.Database.Operations.TaskManagement.TaskQueues;
using Bones.Logic.Features.Initiatives;
using Bones.Shared.Consts;
using Microsoft.AspNetCore.Identity;

namespace Bones.Logic.Features.Tasks.TaskQueues;

/// <inheritdoc />
public class UserHasTaskQueuePermission(UserManager<BonesUser> userManager, RoleManager<BonesRole> roleManager, ISender sender)
    : IRequestHandler<UserHasTaskQueuePermission.Query, QueryResponse<bool>>
{
    /// <summary>
    ///   Checks if the user has permission to do the specified action in the task queue.
    /// </summary>
    /// <param name="TaskQueueId"></param>
    /// <param name="User"></param>
    /// <param name="Claim"></param>
    public sealed record Query(Guid TaskQueueId, BonesUser User, string Claim) : IRequest<QueryResponse<bool>>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Query>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.TaskQueueId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.User).NotNull();
            RuleFor(x => x.Claim).NotEmpty().Custom((claim, ctx) =>
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
        TaskQueue? taskQueue = await sender.Send(new GetTaskQueueByIdDb.Query(request.TaskQueueId), cancellationToken);

        if (taskQueue is null)
        {
            return QueryResponse<bool>.Fail("Task Queue not found");
        }

        bool? initiativePermission = await sender.Send(
            new UserHasInitiativePermission.Query(taskQueue.InitiativeId, request.User, request.Claim),
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

            string neededClaim = BonesClaimTypes.Role.TaskQueue.GetTaskQueueClaimType(taskQueue.Id, request.Claim);

            IList<Claim> claims = await roleManager.GetClaimsAsync(role);
            if (claims.Any(claim => claim.Type == neededClaim && claim.Value == ClaimValues.YES))
            {
                return true;
            }
        }

        return false;
    }
}
