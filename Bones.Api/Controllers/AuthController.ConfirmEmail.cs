using Bones.Api.Features.Auth.ConfirmEmail;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Bones.Api.Controllers;

public partial class AuthController
{
    [AllowAnonymous]
    [HttpPost("confirmEmail", Name = "ConfirmEmailAsync")]
    
    [ProducesResponseType<ContentHttpResult>(StatusCodes.Status200OK, Type = typeof(ContentHttpResult))]
    [ProducesResponseType<UnauthorizedHttpResult>(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedHttpResult))]
    public async Task<Results<ContentHttpResult, UnauthorizedHttpResult>> ConfirmEmailAsync([FromQuery] string userId, [FromQuery] string code, [FromQuery] string? changedEmail)
    {
        QueryResponse<IdentityResult> result = await Sender.Send(new ConfirmEmailQuery()
        {
            UserId = userId,
            Code = code,
            ChangedEmail = changedEmail
        });

        if (result is { Success: true, Result: not null })
        {
            return TypedResults.Text("Thank you for confirming your email.");
        }

        return TypedResults.Unauthorized();
    }
}