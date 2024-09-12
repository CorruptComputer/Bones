using Bones.Api.Models;
using Bones.Shared.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bones.Api.Controllers;

#if DEBUG
/// <summary>
///   Controller for testing stuff
/// </summary>
[AllowAnonymous]
public class TestController(ISender sender) : BonesControllerBase(sender)
{
    /// <summary>
    ///   Throws a BonesException to test the ApiExceptionHandler
    /// </summary>
    /// <returns></returns>
    /// <exception cref="BonesException"></exception>
    [HttpGet("bones-exception", Name = "GetBonesExceptionAsync")]
    [ProducesResponseType<ActionResult<EmptyResponse>>(StatusCodes.Status200OK)]
    public Task<ActionResult> GetBonesExceptionAsync()
    {
        throw new BonesException("Meep Merp");
    }

    /// <summary>
    ///   Throws a ForbiddenAccessException to test the ApiExceptionHandler
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ForbiddenAccessException"></exception>
    [HttpGet("forbidden", Name = "GetForbiddenAsync")]
    [ProducesResponseType<ActionResult<EmptyResponse>>(StatusCodes.Status200OK)]
    public Task<ActionResult> GetForbiddenAsync()
    {
        throw new ForbiddenAccessException();
    }
}
#endif