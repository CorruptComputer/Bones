using System.Security.Claims;
using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.ProjectManagement;
using Bones.Database.Operations.ProjectManagement.Initiatives;
using Bones.Logic.Features.Projects;
using Bones.Shared.Consts;
using Microsoft.AspNetCore.Identity;

namespace Bones.Logic.Features.Initiatives;

/// <inheritdoc />
public sealed class UserIdHasInitiativePermission(UserManager<BonesUser> userManager, RoleManager<BonesRole> roleManager, ISender sender)
    : IRequestHandler<UserIdHasInitiativePermission.Query, QueryResponse<bool>>
{
    /// <summary>
    ///   Checks if the user has permission to do the specified action in the initiative.
    /// </summary>
    /// <param name="InitiativeId"></param>
    /// <param name="UserId"></param>
    /// <param name="Claim"></param>
    public sealed record Query(Guid InitiativeId, Guid UserId, string Claim) : IRequest<QueryResponse<bool>>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Query>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.InitiativeId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.UserId).NotNull().NotEqual(Guid.Empty);
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
        Initiative? initiative = await sender.Send(new GetInitiativesByIdDb.Query(request.InitiativeId), cancellationToken);

        if (initiative is null)
        {
            return QueryResponse<bool>.Fail("Initiative not found");
        }

        bool? projectPermission = await sender.Send(
            new UserIdHasProjectPermission.Query(initiative.Project.Id, request.UserId, request.Claim),
            cancellationToken);

        if (projectPermission == true)
        {
            return true;
        }

        BonesUser? user = await userManager.FindByIdAsync(request.UserId.ToString());
        if (user is null)
        {
            return QueryResponse<bool>.Fail("User not found");
        }

        foreach (string roleName in await userManager.GetRolesAsync(user))
        {
            BonesRole? role = await roleManager.FindByNameAsync(roleName);
            if (role is null)
            {
                return QueryResponse<bool>.Fail("Role not found");
            }

            string neededClaim = BonesClaimTypes.Role.Initiative.GetInitiativeClaimType(initiative.Id, request.Claim);

            IList<Claim> claims = await roleManager.GetClaimsAsync(role);
            if (claims.Any(claim => claim.Type == neededClaim && claim.Value == ClaimValues.YES))
            {
                return true;
            }
        }

        return false;
    }
}
