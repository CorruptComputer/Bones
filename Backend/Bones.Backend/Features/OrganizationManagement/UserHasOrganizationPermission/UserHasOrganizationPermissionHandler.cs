using System.Security.Claims;
using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.OrganizationManagement;
using Bones.Database.Operations.OrganizationManagement.GetOrganizationByIdDb;
using Bones.Shared.Consts;
using Microsoft.AspNetCore.Identity;

namespace Bones.Backend.Features.OrganizationManagement.UserHasOrganizationPermission;

internal sealed class UserHasOrganizationPermissionHandler(UserManager<BonesUser> userManager, RoleManager<BonesRole> roleManager, ISender sender) : IRequestHandler<UserHasOrganizationPermissionQuery, QueryResponse<bool>>
{
    public async Task<QueryResponse<bool>> Handle(UserHasOrganizationPermissionQuery request, CancellationToken cancellationToken)
    {
        BonesOrganization? organization = await sender.Send(new GetOrganizationByIdDbQuery(request.OrganizationId), cancellationToken);

        if (organization is null)
        {
            return QueryResponse<bool>.Fail("Organization not found");
        }
        
        string adminClaim = BonesClaimTypes.Role.Organization.GetOrganizationWideClaimType(organization.Id, BonesClaimTypes.Role.Organization.ORGANIZATION_ADMINISTRATOR);
        string neededClaim = BonesClaimTypes.Role.Organization.GetOrganizationWideClaimType(organization.Id, request.Claim);
        
        foreach (string roleName in await userManager.GetRolesAsync(request.User))
        {
            BonesRole? role = await roleManager.FindByNameAsync(roleName);
            if (role is null)
            {
                return QueryResponse<bool>.Fail("Role not found");
            }
            
            IList<Claim> claims = await roleManager.GetClaimsAsync(role);
            if (claims.Any(claim => (claim.Type == adminClaim || claim.Type == neededClaim) && claim.Value == ClaimValues.YES))
            {
                return true;
            }
        }
        
        return false;
    }
}