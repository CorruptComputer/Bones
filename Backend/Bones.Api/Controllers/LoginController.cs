using System.ComponentModel.DataAnnotations;
using Bones.Api.Models;
using Bones.Database.DbSets.AccountManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
public class LoginController(SignInManager<BonesUser> signInManager, ISender sender) : BonesControllerBase(sender)
{
    /// <summary>
    ///   Request to login
    /// </summary>
    /// <param name="Email">The users email address</param>
    /// <param name="Password">The users password</param>
    /// <param name="TwoFactorCode">If they have 2fa, include the code here</param>
    /// <param name="TwoFactorRecoveryCode">If they have 2fa and can't use their authenticator, include a recovery code here</param>
    public sealed record LoginUserApiRequest([Required] string Email, [Required] string Password, string? TwoFactorCode, string? TwoFactorRecoveryCode);

    /// <summary>
    ///   Logs in a user, returns the active token as a cookie header if successful
    /// </summary>
    /// <param name="login">Request to login</param>
    /// <returns>200 OK if successful, 401 Unauthorized otherwise</returns>
    [HttpPost("login", Name = "LoginAsync")]
    [ProducesResponseType<EmptyResponse>(StatusCodes.Status200OK)]
    [AllowAnonymous]
    public async Task<ActionResult> LoginAsync([FromBody] LoginUserApiRequest login)
    {
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
                Log.Warning("Two-factor code was not provided and is required for login: {Login}", login.Email);
                return Unauthorized(EmptyResponse.Value);
            }
        }

        if (!result.Succeeded)
        {
            Log.Warning("Invalid login attempt: {Login}", login.Email);
            return Unauthorized(EmptyResponse.Value);
        }

        return Ok(EmptyResponse.Value);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpPost("logout", Name = "LogoutAsync")]
    [ProducesResponseType<EmptyResponse>(StatusCodes.Status200OK)]
    [AllowAnonymous]
    public ActionResult LogoutAsync()
    {
        Response.Cookies.Append(".AspNetCore.Identity.Application", string.Empty, new()
        {
            Secure = true,
            HttpOnly = true,
            Expires = DateTimeOffset.Now.AddDays(-1)
        });

        return Ok(EmptyResponse.Value);
    }
}