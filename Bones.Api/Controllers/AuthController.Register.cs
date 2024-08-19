using Bones.Api.Features.Identity.RegisterUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Bones.Api.Controllers;

public sealed partial class AuthController
{
    [AllowAnonymous]
    [HttpPost("register", Name = "RegisterAsync")]
    [ProducesResponseType<Created>(StatusCodes.Status201Created, Type = typeof(Created))]
    [ProducesResponseType<ValidationProblem>(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblem))]
    public async Task<Results<Created, ValidationProblem>> RegisterAsync([FromBody] RegisterUserQuery registration, HttpContext context)
    {
        QueryResponse<IdentityResult> result = await Sender.Send(registration);

        if (result.Success)
        {
            return TypedResults.Created();
        }

        return CreateValidationProblem(result.Result);
    }
}