using System.ComponentModel.DataAnnotations;
using Bones.Backend.Features.AccountManagement.ConfirmEmail;
using Bones.Backend.Features.AccountManagement.QueueConfirmationEmail;
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
/// 
/// </summary>
/// <param name="signInManager"></param>
/// <param name="sender"></param>
/// <remarks>
///   See: https://github.com/dotnet/aspnetcore/blob/main/src/Identity/Core/src/IdentityApiEndpointRouteBuilderExtensions.cs
/// </remarks>
public sealed class AccountManagementController(SignInManager<BonesUser> signInManager, ISender sender) : BonesControllerBase(sender)
{
    public sealed record RegisterUserApiRequest([Required] string Email, [Required] string Password);

    [HttpPost("register", Name = "RegisterAsync")]
    [ProducesResponseType<ActionResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ActionResult<Dictionary<string, string[]>>>(StatusCodes.Status400BadRequest)]
    [AllowAnonymous]
    public async Task<ActionResult> RegisterAsync([FromBody] RegisterUserApiRequest registration)
    {
        QueryResponse<IdentityResult> result = await Sender.Send(new RegisterUserRequest(registration.Email, registration.Password));

        if (!result.Success || !(result.Result?.Succeeded ?? false))
        {
            return BadRequest(ReadErrorsFromIdentityResult(result.Result ?? IdentityResult.Failed()));
        }

        return Ok();
    }

    public sealed record LoginUserApiRequest([Required] string Email, [Required] string Password, string? TwoFactorCode, string? TwoFactorRecoveryCode);

    [HttpPost("login", Name = "LoginAsync")]
    [ProducesResponseType<ActionResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ActionResult>(StatusCodes.Status401Unauthorized)]
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
            return Unauthorized();
        }

        return Ok();
    }

    [HttpGet("confirm-email", Name = "ConfirmEmailAsync")]
    [ProducesResponseType<ActionResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ActionResult>(StatusCodes.Status401Unauthorized)]
    [AllowAnonymous]
    public async Task<ActionResult> ConfirmEmailAsync([FromQuery][Required] Guid userId, [FromQuery][Required] string code, [FromQuery] string? changedEmail)
    {
        QueryResponse<IdentityResult> result = await Sender.Send(new ConfirmEmailRequest(userId, code, changedEmail));

        if (!result.Success || !(result.Result?.Succeeded ?? false))
        {
            return Unauthorized();
        }

        return Ok();
    }

    public sealed record ResendConfirmationEmailApiRequest([Required] string Email);

    [HttpPost("resend-confirmation-email", Name = "ResendConfirmationEmailAsync")]
    [ProducesResponseType<ActionResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ActionResult>(StatusCodes.Status401Unauthorized)]
    [AllowAnonymous]
    public async Task<ActionResult> ResendConfirmationEmailAsync([FromBody][Required] ResendConfirmationEmailApiRequest resendRequest)
    {
        CommandResponse result = await Sender.Send(new ResendConfirmationEmailRequest(resendRequest.Email));

        if (!result.Success)
        {
            return Unauthorized();
        }

        return Ok();
    }


    [HttpPost("logout", Name = "LogoutAsync")]
    [ProducesResponseType<ActionResult>(StatusCodes.Status200OK)]
    [AllowAnonymous]
    public ActionResult LogoutAsync()
    {
        Response.Cookies.Append(".AspNetCore.Identity.Application", string.Empty, new()
        {
            Secure = true,
            HttpOnly = true,
            Expires = DateTimeOffset.Now.AddDays(-1)
        });

        return Ok();
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