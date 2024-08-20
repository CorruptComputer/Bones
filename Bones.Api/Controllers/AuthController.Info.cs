using System.Security.Claims;
using Bones.Api.Features.Auth.GetUserInfo;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace Bones.Api.Controllers;

public partial class AuthController
{
    [HttpGet("info", Name = "GetInfoAsync")]
    [ProducesResponseType<Ok<GetUserInfoResponse>>(StatusCodes.Status200OK, Type = typeof(Ok<GetUserInfoResponse>))]
    [ProducesResponseType<ValidationProblem>(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblem))]
    [ProducesResponseType<NotFound>(StatusCodes.Status404NotFound, Type = typeof(NotFound))]
    public async Task<Results<Ok<GetUserInfoResponse>, ValidationProblem, NotFound>> GetInfoAsync()
    {
        QueryResponse<GetUserInfoResponse> result = await Sender.Send(new GetUserInfoQuery()
        {
            ClaimsPrincipal = User
        });

        if (result is { Success: true, Result: not null })
        {
            return TypedResults.Ok(result.Result);
        }

        return TypedResults.NotFound();
    }
}