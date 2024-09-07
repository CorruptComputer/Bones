using Bones.Database.DbSets.AccountManagement;
using Microsoft.AspNetCore.Identity;

namespace Bones.Backend.Features.AccountManagement.GetUserByClaimsPrincipal;

internal sealed class GetUserByClaimsPrincipalHandler(UserManager<BonesUser> userManager) : IRequestHandler<GetUserByClaimsPrincipalQuery, QueryResponse<BonesUser>>
{
    public async Task<QueryResponse<BonesUser>> Handle(GetUserByClaimsPrincipalQuery request, CancellationToken cancellationToken)
    {
        if (request.ClaimsPrincipal != null)
        {
            return await userManager.GetUserAsync(request.ClaimsPrincipal);
        }

        return QueryResponse<BonesUser>.Fail("Claims Principal was null");
    }
}