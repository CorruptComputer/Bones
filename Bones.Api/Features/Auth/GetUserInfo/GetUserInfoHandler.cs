using Bones.Database.DbSets.Identity;
using Microsoft.AspNetCore.Identity;

namespace Bones.Api.Features.Auth.GetUserInfo;

public class GetUserInfoHandler(UserManager<BonesUser> userManager) : IRequestHandler<GetUserInfoQuery, QueryResponse<GetUserInfoResponse>>
{
    public async Task<QueryResponse<GetUserInfoResponse>> Handle(GetUserInfoQuery request, CancellationToken cancellationToken)
    {
        if (await userManager.GetUserAsync(request.ClaimsPrincipal) is not { } user)
        {
            return new()
            {
                Success = false,
                FailureReason = "User not found"
            };
        }

        return new GetUserInfoResponse()
        {
            Email = await userManager.GetEmailAsync(user) ?? throw new NotSupportedException("Users must have an email."),
            IsEmailConfirmed = await userManager.IsEmailConfirmedAsync(user),
        };
    }
}