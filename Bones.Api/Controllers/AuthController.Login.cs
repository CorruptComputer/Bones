using Bones.Api.Features.Identity.LoginUser;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Bones.Api.Controllers;

public sealed partial class AuthController
{
    [AllowAnonymous]
    [HttpPost("login", Name = "LoginAsync")]
    [ProducesResponseType<Ok<AccessTokenResponse>>(StatusCodes.Status200OK, Type = typeof(Ok<AccessTokenResponse>))]
    [ProducesResponseType<EmptyHttpResult>(StatusCodes.Status200OK, Type = typeof(EmptyHttpResult))]
    [ProducesResponseType<ProblemHttpResult>(StatusCodes.Status401Unauthorized, Type = typeof(ProblemHttpResult))]
    public async Task<Results<Ok<AccessTokenResponse>, EmptyHttpResult, ProblemHttpResult>> LoginAsync([FromBody] LoginUserQuery login, [FromQuery] bool? useCookies, [FromQuery] bool? useSessionCookies)
    {
        login.UseCookies = useCookies;
        login.UseSessionCookies = useSessionCookies;
        
        QueryResponse<SignInResult> result = await Sender.Send(login);

        if (!result.Success)
        {
            return TypedResults.Problem(result.ToString(), statusCode: StatusCodes.Status401Unauthorized);
        }

        // The signInManager already produced the needed response in the form of a cookie or bearer token.
        return TypedResults.Empty;
    }
}