using Bones.Database.DbSets.Identity;
using Microsoft.AspNetCore.Identity;

namespace Bones.Api.Features.Identity.LoginUser;

public class LoginUserHandler(SignInManager<BonesUser> signInManager) : IRequestHandler<LoginUserQuery, QueryResponse<SignInResult>>
{
    public async Task<QueryResponse<SignInResult>> Handle(LoginUserQuery request, CancellationToken cancellationToken)
    {
        bool useCookieScheme = (request.UseCookies == true) || (request.UseSessionCookies == true);
        bool isPersistent = (request.UseCookies == true) && (request.UseSessionCookies != true);
        signInManager.AuthenticationScheme = useCookieScheme ? IdentityConstants.ApplicationScheme : IdentityConstants.BearerScheme;

        SignInResult result = await signInManager.PasswordSignInAsync(request.Email, request.Password, isPersistent, lockoutOnFailure: true);

        if (result.RequiresTwoFactor)
        {
            if (!string.IsNullOrEmpty(request.TwoFactorCode))
            {
                result = await signInManager.TwoFactorAuthenticatorSignInAsync(request.TwoFactorCode, isPersistent, rememberClient: isPersistent);
            }
            else if (!string.IsNullOrEmpty(request.TwoFactorRecoveryCode))
            {
                result = await signInManager.TwoFactorRecoveryCodeSignInAsync(request.TwoFactorRecoveryCode);
            }
        }

        if (!result.Succeeded)
        {
            return new()
            {
                Success = false,
                FailureReason = "Login failed.",
                Result = result
            };
        }

        return result;
    }
}