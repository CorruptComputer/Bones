using System.Security.Claims;
using Bones.Backend.Features.OrganizationManagement.UserHasOrganizationPermission;
using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.ProjectManagement;
using Bones.Database.Operations.ProjectManagement.Projects.GetProjectByIdDb;
using Bones.Shared.Backend.Enums;
using Bones.Shared.Consts;
using Microsoft.AspNetCore.Identity;

namespace Bones.Backend.Features.ProjectManagement.Projects.UserHasProjectPermission;

internal sealed class UserHasProjectPermissionHandler(UserManager<BonesUser> userManager, RoleManager<BonesRole> roleManager, ISender sender) : IRequestHandler<UserHasProjectPermissionQuery, QueryResponse<bool>>
{
    public async Task<QueryResponse<bool>> Handle(UserHasProjectPermissionQuery request, CancellationToken cancellationToken)
    {
        Project? project = await sender.Send(new GetProjectByIdDbQuery(request.ProjectId), cancellationToken);

        if (project is null)
        {
            return QueryResponse<bool>.Fail("Project not found");
        }

        if (project.OwnerType == OwnershipType.User || project.OwningOrganization == null)
        {
            return project.OwningUser?.Id == request.User.Id;
        }

        bool? organizationPermission = await sender.Send(
            new UserHasOrganizationPermissionQuery(project.OwningOrganization.Id, request.User, request.Claim),
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

            string neededClaim = BonesClaimTypes.Role.Organization.Project.GetProjectClaimType(project.Id, request.Claim);

            IList<Claim> claims = await roleManager.GetClaimsAsync(role);
            if (claims.Any(claim => claim.Type == neededClaim && claim.Value == ClaimValues.YES))
            {
                return true;
            }
        }

        return false;
    }
}