using System.Security.Claims;
using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.ProjectManagement;
using Bones.Database.Operations.ProjectManagement.Projects;
using Bones.Logic.Features.Organizations;
using Bones.Shared.Backend.Enums;
using Bones.Shared.Consts;
using Microsoft.AspNetCore.Identity;

namespace Bones.Logic.Features.Projects;

/// <inheritdoc />
public sealed class UserIdHasProjectPermission(UserManager<BonesUser> userManager, RoleManager<BonesRole> roleManager, ISender sender)
    : IRequestHandler<UserIdHasProjectPermission.Query, QueryResponse<bool>>
{
    /// <summary>
    ///   Checks if the user has permission to do the specified action in the project.
    /// </summary>
    /// <param name="ProjectId"></param>
    /// <param name="UserId"></param>
    /// <param name="Claim"></param>
    public sealed record Query(Guid ProjectId, Guid UserId, string Claim) : IRequest<QueryResponse<bool>>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Query>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.ProjectId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.UserId).NotNull().NotEqual(Guid.Empty);
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

        if (project.OwnerType == OwnershipType.User || project.OwningOrganization == null)
        {
            return project.OwningUser?.Id == request.UserId;
        }

        bool? organizationPermission = await sender.Send(
            new UserIdHasOrganizationPermission.Query(project.OwningOrganization.Id, request.UserId, request.Claim),
            cancellationToken);

        if (organizationPermission == true)
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

            string neededClaim = BonesClaimTypes.Role.Project.GetProjectClaimType(project.Id, request.Claim);

            IList<Claim> claims = await roleManager.GetClaimsAsync(role);
            if (claims.Any(claim => claim.Type == neededClaim && claim.Value == ClaimValues.YES))
            {
                return true;
            }
        }

        return false;
    }
}
