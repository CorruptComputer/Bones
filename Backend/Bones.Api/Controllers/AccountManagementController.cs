using System.ComponentModel.DataAnnotations;
using Bones.Api.Models;
using Bones.Backend.Features.AccountManagement.ConfirmEmail;
using Bones.Backend.Features.AccountManagement.GetUserByClaimsPrincipal;
using Bones.Backend.Features.AccountManagement.RegisterUser;
using Bones.Database.DbSets.AccountManagement;
using Bones.Shared.Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ResendConfirmationEmailRequest = Bones.Backend.Features.AccountManagement.ResendConfirmationEmail.ResendConfirmationEmailRequest;
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
public sealed class AccountManagementController(SignInManager<BonesUser> signInManager, ISender sender) : BonesControllerBase(sender)
{
    /// <summary>
    ///   Request to register a new user
    /// </summary>
    /// <param name="Email">Email, must be valid and unique</param>
    /// <param name="Password">Password, must pass validation (1 upper, 1 lower, 1 number, 1 special character, and at least 8 characters long)</param>
    public sealed record RegisterUserApiRequest([Required] string Email, [Required] string Password);

    /// <summary>
    ///   Registers a new user if all validations pass
    /// </summary>
    /// <param name="registration">Request to register a new user</param>
    /// <returns>200 OK if created, 400 BadRequest otherwise with the reason why its failing</returns>
    [HttpPost("register", Name = "RegisterAsync")]
    [ProducesResponseType<ActionResult<EmptyResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ActionResult<Dictionary<string, string[]>>>(StatusCodes.Status400BadRequest)]
    [AllowAnonymous]
    public async Task<ActionResult> RegisterAsync([FromBody] RegisterUserApiRequest registration)
    {
        QueryResponse<IdentityResult> result = await Sender.Send(new RegisterUserRequest(registration.Email, registration.Password));

        if (!result.Success || !(result.Result?.Succeeded ?? false))
        {
            return BadRequest(ReadErrorsFromIdentityResult(result.Result ?? IdentityResult.Failed()));
        }

        return Ok(EmptyResponse.Value);
    }

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
    [ProducesResponseType<ActionResult<EmptyResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ActionResult<EmptyResponse>>(StatusCodes.Status401Unauthorized)]
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
        }

        if (!result.Succeeded)
        {
            return Unauthorized(EmptyResponse.Value);
        }

        return Ok(EmptyResponse.Value);
    }

    /// <summary>
    ///   Confirms a users email address
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="code"></param>
    /// <param name="changedEmail"></param>
    /// <returns></returns>
    [HttpGet("confirm-email", Name = "ConfirmEmailAsync")]
    [ProducesResponseType<ActionResult<EmptyResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ActionResult<EmptyResponse>>(StatusCodes.Status401Unauthorized)]
    [AllowAnonymous]
    public async Task<ActionResult> ConfirmEmailAsync([FromQuery][Required] Guid userId, [FromQuery][Required] string code, [FromQuery] string? changedEmail)
    {
        QueryResponse<IdentityResult> result = await Sender.Send(new ConfirmEmailRequest(userId, code, changedEmail));

        if (!result.Success || !(result.Result?.Succeeded ?? false))
        {
            return Unauthorized(EmptyResponse.Value);
        }

        return Ok(EmptyResponse.Value);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Email"></param>
    public sealed record ResendConfirmationEmailApiRequest([Required] string Email);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="resendRequest"></param>
    /// <returns></returns>
    [HttpPost("resend-confirmation-email", Name = "ResendConfirmationEmailAsync")]
    [ProducesResponseType<ActionResult<EmptyResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ActionResult<EmptyResponse>>(StatusCodes.Status401Unauthorized)]
    [AllowAnonymous]
    public async Task<ActionResult> ResendConfirmationEmailAsync([FromBody][Required] ResendConfirmationEmailApiRequest resendRequest)
    {
        CommandResponse result = await Sender.Send(new ResendConfirmationEmailRequest(resendRequest.Email));

        if (!result.Success)
        {
            return Unauthorized(EmptyResponse.Value);
        }

        return Ok(EmptyResponse.Value);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpPost("logout", Name = "LogoutAsync")]
    [ProducesResponseType<ActionResult<EmptyResponse>>(StatusCodes.Status200OK)]
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Email"></param>
    /// <param name="DisplayName"></param>
    public record GetMyBasicInfoResponse(string? Email, string? DisplayName);

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet("my/basic-info", Name = "GetMyBasicInfoAsync")]
    [ProducesResponseType<ActionResult<GetMyBasicInfoResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ActionResult<EmptyResponse>>(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> GetMyBasicInfoAsync()
    {
        QueryResponse<BonesUser> user = await Sender.Send(new GetUserByClaimsPrincipalRequest(User));

        if (!user.Success || user.Result == null)
        {
            return Unauthorized(EmptyResponse.Value);
        }

        return Ok(new GetMyBasicInfoResponse(user.Result.Email, user.Result.DisplayName));
    }

    private static Dictionary<string, string[]> ReadErrorsFromIdentityResult(IdentityResult result)
    {
        Dictionary<string, string[]> errorDictionary = new();

        foreach (IdentityError error in result.Errors)
        {
            string[] newDescriptions;

            if (errorDictionary.TryGetValue(error.Code, out string[]? descriptions))
            {
                newDescriptions = new string[descriptions.Length + 1];
                Array.Copy(descriptions, newDescriptions, descriptions.Length);
                newDescriptions[descriptions.Length] = error.Description;
            }
            else
            {
                newDescriptions = [error.Description];
            }

            errorDictionary[error.Code] = newDescriptions;
        }

        return errorDictionary;
    }
}