using System.Security.Claims;
using Bones.Api.Features.Identity.RefreshToken;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Bones.Api.Controllers;

public sealed partial class AuthController
{
    [AllowAnonymous]
    [HttpPost("refresh", Name = "RefreshAsync")]
    [ProducesResponseType<Ok<AccessTokenResponse>>(StatusCodes.Status200OK, Type = typeof(Ok<AccessTokenResponse>))]
    [ProducesResponseType<UnauthorizedHttpResult>(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedHttpResult))]
    [ProducesResponseType<SignInHttpResult>(StatusCodes.Status401Unauthorized, Type = typeof(SignInHttpResult))]
    [ProducesResponseType<ChallengeHttpResult>(StatusCodes.Status401Unauthorized, Type = typeof(ChallengeHttpResult))]
    public async Task<Results<
        Ok<AccessTokenResponse>, 
        UnauthorizedHttpResult, 
        SignInHttpResult, 
        ChallengeHttpResult>> RefreshAsync([FromBody] RefreshTokenQuery refreshRequest)
    {
        QueryResponse<ClaimsPrincipal> result = await Sender.Send(refreshRequest);

        if (result is { Success: true, Result: not null })
        {
            return TypedResults.SignIn(result.Result, authenticationScheme: IdentityConstants.BearerScheme);
        }

        return TypedResults.Challenge();
    }
}