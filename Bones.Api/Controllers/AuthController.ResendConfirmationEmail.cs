using Bones.Api.Features.Auth.ResendConfirmationEmail;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Bones.Api.Controllers;

public partial class AuthController
{
    [AllowAnonymous]
    [HttpPost("resendConfirmationEmail", Name = "ResendConfirmationEmailAsync")]
    [ProducesResponseType<Ok>(StatusCodes.Status200OK, Type = typeof(Ok))]
    public async Task<Ok> ResendConfirmationEmailAsync([FromBody] ResendConfirmationEmailCommand resendRequest)
    {
        resendRequest.Context = HttpContext;
        await Sender.Send(resendRequest);

        return TypedResults.Ok();
    }
}