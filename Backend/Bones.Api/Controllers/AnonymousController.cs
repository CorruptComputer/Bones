using System.ComponentModel.DataAnnotations;
using Bones.Api.Models;
using Bones.Backend.Features.Accounts.ConfirmEmail;
using Bones.Backend.Features.Accounts.QueueForgotPasswordEmail;
using Bones.Backend.Features.Accounts.QueueResendConfirmationEmail;
using Bones.Backend.Features.Accounts.RegisterUser;
using Bones.Shared.Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Bones.Api.Controllers;

/// <summary>
///   Handles everything related to User Accounts
/// </summary>
/// <param name="sender">MediatR sender</param>
[AllowAnonymous]
public sealed partial class AnonymousController(ISender sender) : BonesControllerBase(sender)
{
    /// <summary>
    ///   Registers a new user if all validations pass
    /// </summary>
    /// <param name="registration">Request to register a new user</param>
    /// <returns>200 OK if created, 400 BadRequest otherwise with the reason why its failing</returns>
    [HttpPost("register", Name = "RegisterAsync")]
    [ProducesResponseType<EmptyResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<Dictionary<string, string[]>>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> RegisterAsync([FromBody] RegisterUserApiRequest registration)
    {
        QueryResponse<IdentityResult> result = await Sender.Send(new RegisterUserQuery(registration.Email, registration.Password));

        if (!result.Success || !(result.Result?.Succeeded ?? false))
        {
            return BadRequest(ReadErrorsFromIdentityResult(result.Result ?? IdentityResult.Failed()));
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
    [ProducesResponseType<EmptyResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult> ConfirmEmailAsync([FromQuery][Required] Guid userId, [FromQuery][Required] string code, [FromQuery] string? changedEmail)
    {
        QueryResponse<IdentityResult> result = await Sender.Send(new ConfirmEmailQuery(userId, code, changedEmail));

        if (!result.Success || !(result.Result?.Succeeded ?? false))
        {
            return Unauthorized(EmptyResponse.Value);
        }

        return Ok(EmptyResponse.Value);
    }

    /// <summary>
    ///   Re-queues the confirmation email to send
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    [HttpPost("resend-confirmation-email", Name = "ResendConfirmationEmailAsync")]
    [ProducesResponseType<EmptyResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult> ResendConfirmationEmailAsync([FromQuery][Required] string email)
    {
        CommandResponse result = await Sender.Send(new QueueResendConfirmationEmailCommand(email));

        if (!result.Success)
        {
            return Unauthorized(EmptyResponse.Value);
        }

        return Ok(EmptyResponse.Value);
    }

    /// <summary>
    ///   Queues a forgot password email
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    [HttpPost("forgot-password", Name = "ForgotPasswordAsync")]
    [ProducesResponseType<EmptyResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult> ForgotPasswordAsync([FromQuery][Required] string email)
    {
        await Sender.Send(new QueueForgotPasswordEmailCommand(email));

        return Ok(EmptyResponse.Value);
    }
}