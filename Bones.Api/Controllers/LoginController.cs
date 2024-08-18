using Bones.Api.Features.Identity;
using Bones.Database.DbSets.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bones.Api.Controllers;

public class LoginController(BonesUser? user, ISender sender) : BonesControllerBase(sender)
{
    
    [HttpGet("whoami", Name = "WhoAmI")]
    [ProducesResponseType(typeof(ActionResult<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ActionResult<string>), StatusCodes.Status400BadRequest)]
    public ActionResult<string> WhoAmI()
    {
        return Ok(user?.UserName ?? null);
    }

    public record CreateAccountRequest(string Email, string Password);
    
    /// <summary>
    ///     Creates a new account.
    /// </summary>
    /// <param name="request">The email address and password for the new account.</param>
    /// <returns>Created if created, otherwise BadRequest with a message of what went wrong.</returns>
    [AllowAnonymous]
    [HttpPost("create", Name = "CreateAccountAsync")]
    [ProducesResponseType(typeof(ActionResult), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<string>> CreateAccountAsync([FromBody] CreateAccountRequest request)
    {
        CommandResponse response = await Sender.Send(new CreateUser.Command(request.Email, request.Password));
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
        CommandResponse response = await Sender.Send(new VerifyUserEmail.Command(accountId, token));
        if (!response.Success)
        {
            return BadRequest(response.FailureReason);
        }

        return Ok();
    }
    
    public ActionResult<string> Login(string email, string password)
    {
        return Ok("");
    }
}