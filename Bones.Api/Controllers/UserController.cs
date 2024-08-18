using Bones.Api.Features.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Bones.Api.Controllers;

/// <summary>
///     Manages API functions relating to accounts.
/// </summary>
/// <param name="sender">MediatR sender to communicate with the back-end.</param>
public class UserController(ISender sender) : BonesControllerBase(sender)
{


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
        CommandResponse response = await Sender.Send(new ChangeUserEmail.Command(accountId, email));
        if (!response.Success)
        {
            return BadRequest(response.FailureReason);
        }

        return Accepted();
    }
}