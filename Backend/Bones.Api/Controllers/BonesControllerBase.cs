using System.Net.Mime;
using Bones.Api.Models;
using Bones.Backend.Features.AccountManagement.GetUserByClaimsPrincipal;
using Bones.Database.DbSets.AccountManagement;
using Bones.Shared.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bones.Api.Controllers;

/// <summary>
///     Base controller for the Bones API, everything should extend from this.
/// </summary>
/// <param name="sender">MediatR sender</param>
[Authorize]
[ApiController]
[Route("[controller]")]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType<ActionResult<EmptyResponse>>(StatusCodes.Status401Unauthorized)]
[ProducesResponseType<ActionResult<EmptyResponse>>(StatusCodes.Status403Forbidden)]
[ProducesResponseType<ActionResult<EmptyResponse>>(StatusCodes.Status500InternalServerError)]
public class BonesControllerBase(ISender sender) : ControllerBase
{
    /// <summary>
    ///     MediatR sender for commands and queries to the backend, you can technically send things straight to the DB here,
    ///     but don't do that.
    /// </summary>
    protected ISender Sender => sender;

    /// <summary>
    ///   Gets the user for the current request
    /// </summary>
    /// <returns></returns>
    protected async Task<BonesUser> GetCurrentBonesUserAsync()
    {
        BonesUser? user = await Sender.Send(new GetUserByClaimsPrincipalQuery(User));
        
        // I don't think this should really happen, if their claims principal was invalid the auth should have stopped the request already
        return user ?? throw new ForbiddenAccessException();
    }
}