using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Bones.Api.Controllers.Base;
using Bones.Api.Models.Anonymous;
using Bones.Logic.Features.Accounts;
using Bones.Logic.Features.System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Bones.Api.Controllers;

/// <summary>
///   Handles everything related to User Accounts
/// </summary>
/// <param name="sender">MediatR sender</param>
/// <param name="config">System config</param>
[AllowAnonymous]
public sealed class AnonymousController(ISender sender, BonesBackendConfiguration config) : BonesControllerBase(sender)
{
    /// <summary>
    ///   Registers a new user if all validations pass
    /// </summary>
    /// <param name="registration">Request to register a new user</param>
    /// <returns>200 OK if created, 400 BadRequest otherwise with the reason why its failing</returns>
    [HttpPost("register", Name = "RegisterAsync")]
    [ProducesResponseType<EmptyResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<EmptyResponse>> RegisterAsync([FromBody] RegisterUserApiRequest registration)
    {
        QueryResponse<IdentityResult> result = await Sender.Send(new RegisterUser.Query(registration.Email, registration.Password));

        if (!result.Success || !(result.Result?.Succeeded ?? false))
        {
            return BadRequest(ErrorResponse.FromIdentityResult(result.Result ?? IdentityResult.Failed()));
        }

        return EmptyResponse.Value;
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
    public async ValueTask<ActionResult<EmptyResponse>> ConfirmEmailAsync([FromQuery][Required] Guid userId, [FromQuery][Required] string code, [FromQuery] string? changedEmail)
    {
        QueryResponse<IdentityResult> result = await Sender.Send(new ConfirmEmail.Query(userId, code, changedEmail));

        if (!result.Success || !(result.Result?.Succeeded ?? false))
        {
            return Unauthorized(EmptyResponse.Value);
        }

        return EmptyResponse.Value;
    }

    /// <summary>
    ///   Re-queues the confirmation email to send
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    [HttpPost("resend-confirmation-email", Name = "ResendConfirmationEmailAsync")]
    [ProducesResponseType<EmptyResponse>(StatusCodes.Status200OK)]
    public async ValueTask<ActionResult<EmptyResponse>> ResendConfirmationEmailAsync([FromQuery][Required] string email)
    {
        CommandResponse result = await Sender.Send(new QueueResendConfirmationEmail.Command(email));

        if (!result.Success)
        {
            return Unauthorized(EmptyResponse.Value);
        }

        return EmptyResponse.Value;
    }

    /// <summary>
    ///   Queues a forgot password email
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    [HttpPost("forgot-password", Name = "ForgotPasswordAsync")]
    [ProducesResponseType<EmptyResponse>(StatusCodes.Status200OK)]
    public async ValueTask<ActionResult<EmptyResponse>> ForgotPasswordAsync([FromQuery][Required] string email)
    {
        await Sender.Send(new QueueForgotPasswordEmail.Command(email));

        return EmptyResponse.Value;
    }

    /// <summary>
    ///   Gets the WebUI's basic configuration
    /// </summary>
    /// <returns></returns>
    [HttpGet("web-config", Name = "GetApiConfigAsync")]
    [ProducesResponseType<ApiConfigResponse>(StatusCodes.Status200OK)]
    public ActionResult<ApiConfigResponse> GetApiConfigAsync()
    {
        return new ApiConfigResponse()
        {
            SetupForTesting = config.SetupForTesting,
            ApiVersion = Assembly.GetEntryAssembly()?
                                 .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                                 // For example: 0.0.1+b9d1873a
                                 ?.InformationalVersion.Split('+')[1] ?? "ERROR"
        };
    }
}