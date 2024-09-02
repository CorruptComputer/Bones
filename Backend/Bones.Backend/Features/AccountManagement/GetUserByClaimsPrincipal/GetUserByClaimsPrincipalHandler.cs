using Bones.Database.DbSets.AccountManagement;
using Microsoft.AspNetCore.Identity;

namespace Bones.Backend.Features.AccountManagement.GetUserByClaimsPrincipal;

internal sealed class GetUserByClaimsPrincipalHandler(UserManager<BonesUser> userManager) : IRequestHandler<GetUserByClaimsPrincipalRequest, QueryResponse<BonesUser>>
{
    public async Task<QueryResponse<BonesUser>> Handle(GetUserByClaimsPrincipalRequest request, CancellationToken cancellationToken)
    {
        if (request.ClaimsPrincipal != null)
        {
            return await userManager.GetUserAsync(request.ClaimsPrincipal);
        }

        return new()
        {
            Success = false,
            FailureReason = "Claims Principal was null"
        };
    }
}