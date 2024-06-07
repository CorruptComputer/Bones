using Bones.Backend.Features.Accounts;
using Bones.Backend.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Bones.Api.Controllers;

/// <summary>
///     Manages API functions relating to accounts.
/// </summary>
/// <param name="sender">MediatR sender to communicate with the back-end.</param>
public class AccountController(ISender sender) : BonesControllerBase(sender)
{
    /// <summary>
    ///     Creates a new account.
    /// </summary>
    /// <param name="email">The email address for the new account.</param>
    /// <returns>Created if created, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpPost("create/{email}", Name = "CreateAccountAsync")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateAccountAsync(string email)
    {
        BackendCommandResponse response = await Sender.Send(new CreateAccountCommand(email));
        if (!response.Success)
        {
            return BadRequest(response.FailureReason);
        }

        return Created();
    }

    /// <summary>
    ///     Verify the account email.
    /// </summary>
    /// <param name="accountId">Account ID that is being verified</param>
    /// <param name="token">Please drink verification can.</param>
    /// <returns>OK if verified, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpPut("{accountId:guid}/verify-email/{token:guid}", Name = "VerifyAccountEmailAsync")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> VerifyAccountEmailAsync(Guid accountId, Guid token)
    {
        BackendCommandResponse response = await Sender.Send(new VerifyAccountEmailCommand(accountId, token));
        if (!response.Success)
        {
            return BadRequest(response.FailureReason);
        }

        return Ok();
    }

    /// <summary>
    ///     Change the email address on the account.
    /// </summary>
    /// <param name="accountId">Account ID to change the email on.</param>
    /// <param name="email">New email address for the account.</param>
    /// <returns>Accepted if changed, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpPost("{accountId:guid}/change-email/{email}", Name = "ChangeAccountEmailAsync")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> ChangeAccountEmailAsync(Guid accountId, string email)
    {
        BackendCommandResponse response = await Sender.Send(new ChangeAccountEmailCommand(accountId, email));
        if (!response.Success)
        {
            return BadRequest(response.FailureReason);
        }

        return Accepted();
    }
}