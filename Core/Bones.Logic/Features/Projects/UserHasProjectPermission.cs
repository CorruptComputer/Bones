using System.Security.Claims;
using Bones.Database.DbSets.Accounts;
using Bones.Database.DbSets.Projects;
using Bones.Database.Operations.Projects.Projects;
using Bones.Logic.Features.Organizations;
using Bones.Shared.Consts;
using Microsoft.AspNetCore.Identity;

namespace Bones.Logic.Features.Projects;

/// <inheritdoc />
public sealed class UserHasProjectPermission(UserManager<BonesUser> userManager, RoleManager<BonesRole> roleManager, ISender sender)
    : IRequestHandler<UserHasProjectPermission.Query, QueryResponse<bool>>
{
    /// <summary>
    ///   Checks if the user has permission to do the specified action in the project.
    /// </summary>
    /// <param name="ProjectId"></param>
    /// <param name="User"></param>
    /// <param name="Claim"></param>
    public sealed record Query(Guid ProjectId, BonesUser User, string Claim) : IRequest<QueryResponse<bool>>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Query>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.ProjectId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.User).NotNull();
            RuleFor(x => x.Claim).NotEmpty().Custom((claim, ctx) =>
            {
                if (claim.Contains('|'))
                {
                    ctx.AddFailure("Claim contains '|', this means you probably called GetProjectClaimType(). Don't do that, just pass in the claim name.");
                }
            });
        }
    }

    /// <inheritdoc />
    public async Task<QueryResponse<bool>> Handle(Query request, CancellationToken cancellationToken)
    {
        Project? project = await sender.Send(new GetProjectByIdDb.Query(request.ProjectId), cancellationToken);

        if (project is null)
        {
            return QueryResponse<bool>.Fail("Project not found");
        }

        if (project.IsUserOwned)
        {
            return project.OwningUserId == request.User.Id;
        }

        if (project.IsOrganizationOwned)
        {
            bool? organizationPermission = await sender.Send(
            new UserHasOrganizationPermission.Query(project.OwningOrganizationId.Value, request.User, request.Claim),
            cancellationToken);

            if (organizationPermission == true)
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

                string neededClaim = BonesClaimTypes.Role.Project.GetProjectClaimType(project.Id, request.Claim);

                IList<Claim> claims = await roleManager.GetClaimsAsync(role);
                if (claims.Any(claim => claim.Type == neededClaim && claim.Value == ClaimValues.YES))
                {
                    return true;
                }
            }
        }

        return false;
    }
}