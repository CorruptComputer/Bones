using Bones.Api.Controllers.Base;
using Bones.Api.Models.Login;
using Bones.Database.DbSets.AccountManagement;
using Bones.Logic.Features.Accounts;
using Bones.Logic.Features.Audits;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Bones.Api.Controllers;

/// <summary>
///   Handles everything related to User Accounts
/// </summary>
/// <param name="signInManager">Sign in manager, registered and managed by AspNetCore Identity</param>
/// <param name="sender">MediatR sender</param>
/// <remarks>
///   Created using this as a reference:
///   https://github.com/dotnet/aspnetcore/blob/main/src/Identity/Core/src/IdentityApiEndpointRouteBuilderExtensions.cs
/// </remarks>
public sealed class LoginController(SignInManager<BonesUser> signInManager, ISender sender) : AuthenticatedControllerBase(sender)
{
    /// <summary>
    ///   Logs in a user, returns the active token as a cookie header if successful
    /// </summary>
    /// <param name="login">Request to login</param>
    /// <returns>200 OK if successful, 401 Unauthorized otherwise</returns>
    [HttpPost("login", Name = "LoginAsync")]
    [ProducesResponseType<EmptyResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<EmptyResponse>(StatusCodes.Status400BadRequest)]
    [AllowAnonymous]
    public async ValueTask<ActionResult<EmptyResponse>> LoginAsync([FromBody] LoginUserApiRequest login)
    {
        bool? ipCanAttemptLogin = await Sender.Send(new CheckLoginRateLimit.Query(RequestingIpAddress));
        bool? passwordIsExpired = await Sender.Send(new IsPasswordExpired.Query(login.Email));
        if (ipCanAttemptLogin != true || passwordIsExpired == true)
        {
            await Sender.Send(new AddLoginAudit.Command(login.Email, false, RequestingIpAddress));
            return BadRequest(EmptyResponse.Value);
        }

        signInManager.AuthenticationScheme = IdentityConstants.ApplicationScheme;

        SignInResult result = await signInManager.PasswordSignInAsync(login.Email, login.Password, isPersistent: true, lockoutOnFailure: true);

        if (result.RequiresTwoFactor)
        {
            if (!string.IsNullOrEmpty(login.TwoFactorCode))
            {
                result = await signInManager.TwoFactorAuthenticatorSignInAsync(login.TwoFactorCode, isPersistent: true, rememberClient: true);
            }
            else if (!string.IsNullOrEmpty(login.TwoFactorRecoveryCode))
            {
                result = await signInManager.TwoFactorRecoveryCodeSignInAsync(login.TwoFactorRecoveryCode);
            }
            else
            {
                Log.Warning("Two-factor code was not provided and is required for login: {Login} | From IP Address: {IPAddress}", login.Email, RequestingIpAddress);
                await Sender.Send(new AddLoginAudit.Command(login.Email, false, RequestingIpAddress));
                return BadRequest(EmptyResponse.Value);
            }
        }

        await Sender.Send(new AddLoginAudit.Command(login.Email, result.Succeeded, RequestingIpAddress));

        if (!result.Succeeded)
        {
            Log.Warning("Invalid login attempt on account: {Login} | From IP Address: {IPAddress}", login.Email, RequestingIpAddress);
            return BadRequest(EmptyResponse.Value);
        }

        // signInManager sets the cookie header, so there isn't anything for us to do here
        return EmptyResponse.Value;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpPost("logout", Name = "LogoutAsync")]
    [ProducesResponseType<EmptyResponse>(StatusCodes.Status200OK)]
    [AllowAnonymous]
    public ActionResult<EmptyResponse> LogoutAsync()
    {
        Response.Cookies.Append(".AspNetCore.Identity.Application", string.Empty, new()
        {
            Secure = true,
            HttpOnly = true,
            Expires = DateTimeOffset.Now.AddDays(-1)
        });

        return EmptyResponse.Value;
    }
}