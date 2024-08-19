using System.Security.Claims;
using Bones.Database.DbSets.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Bones.Api.Features.Identity.RefreshToken;

public class RefreshTokenHandler(SignInManager<BonesUser> signInManager, IOptionsMonitor<BearerTokenOptions> bearerTokenOptions, TimeProvider timeProvider) : IRequestHandler<RefreshTokenQuery, QueryResponse<ClaimsPrincipal>>
{
    public async Task<QueryResponse<ClaimsPrincipal>> Handle(RefreshTokenQuery request, CancellationToken cancellationToken)
    {
        ISecureDataFormat<AuthenticationTicket> refreshTokenProtector = bearerTokenOptions.Get(IdentityConstants.BearerScheme).RefreshTokenProtector;
        AuthenticationTicket? refreshTicket = refreshTokenProtector.Unprotect(request.RefreshToken);

        // Reject the /refresh attempt with a 401 if the token expired or the security stamp validation fails
        if (refreshTicket?.Properties?.ExpiresUtc is not { } expiresUtc ||
            timeProvider.GetUtcNow() >= expiresUtc ||
            await signInManager.ValidateSecurityStampAsync(refreshTicket.Principal) is not BonesUser user)

        {
            return new()
            {
                Success = false,
                FailureReason = "Invalid refresh token"
            };
        }

        return await signInManager.CreateUserPrincipalAsync(user);
    }
}