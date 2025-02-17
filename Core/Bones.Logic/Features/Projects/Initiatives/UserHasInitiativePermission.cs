using System.Security.Claims;
using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.ProjectManagement;
using Bones.Database.Operations.ProjectManagement.Initiatives;
using Bones.Logic.Features.Projects.Projects;
using Bones.Shared.Consts;
using Microsoft.AspNetCore.Identity;

namespace Bones.Logic.Features.Projects.Initiatives;

/// <inheritdoc />
public sealed class UserHasInitiativePermission(UserManager<BonesUser> userManager, RoleManager<BonesRole> roleManager, ISender sender)
    : IRequestHandler<UserHasInitiativePermission.Query, QueryResponse<bool>>
{
    /// <summary>
    ///   Checks if the user has permission to do the specified action in the initiative.
    /// </summary>
    /// <param name="InitiativeId"></param>
    /// <param name="User"></param>
    /// <param name="Claim"></param>
    public sealed record Query(Guid InitiativeId, BonesUser User, string Claim) : IRequest<QueryResponse<bool>>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Query>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.InitiativeId).NotNull().NotEqual(Guid.Empty);
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
        Initiative? initiative = await sender.Send(new GetInitiativesByIdDb.Query(request.InitiativeId), cancellationToken);

        if (initiative is null)
        {
            return QueryResponse<bool>.Fail("Initiative not found");
        }

        bool? projectPermission = await sender.Send(
            new UserHasProjectPermission.Query(initiative.Project.Id, request.User, request.Claim),
            cancellationToken);

        if (projectPermission == true)
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