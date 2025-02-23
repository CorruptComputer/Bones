using System.Net.Mime;
using Bones.Logic.Features.Accounts;
using Bones.Database.DbSets.AccountManagement;
using Bones.Shared.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Net;

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
[ProducesResponseType<UnauthorizedResult>(StatusCodes.Status401Unauthorized)] // Returned by the [Authorize] attribute
[ProducesResponseType<ErrorResponse>(StatusCodes.Status401Unauthorized)] // Returned by UnauthenticatedException
[ProducesResponseType<ErrorResponse>(StatusCodes.Status403Forbidden)] // Returned by ForbiddenException
[ProducesResponseType<ErrorResponse>(StatusCodes.Status500InternalServerError)] // Returned by any other exception
public class BonesControllerBase(ISender sender) : ControllerBase
{
    /// <summary>
    ///     MediatR sender for commands and queries to the Logic layer,
    ///     you can technically send things straight to the DB here, but don't do that.
    /// </summary>
    protected ISender Sender => sender;

    /// <summary>
    ///   Gets the IP address of the client making the request, or IPAddress.None if it can't be determined.
    /// </summary>
    protected IPAddress RequestingIpAddress => Request.HttpContext.Connection.RemoteIpAddress ?? IPAddress.None;

    /// <summary>
    ///   Gets the user for the current request
    /// </summary>
    /// <returns></returns>
    protected async Task<BonesUser> GetCurrentBonesUserAsync()
    {
        BonesUser? user = await Sender.Send(new GetUserByClaimsPrincipal.Query(User));

        return user ?? throw new UnauthenticatedException();
    }

    /// <summary>
    ///   Gets the errors from an IdentityResult in a format that we can return.
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    protected static Dictionary<string, string[]> ReadErrorsFromIdentityResult(IdentityResult result)
    {
        Dictionary<string, string[]> errorDictionary = [];

        foreach (IdentityError error in result.Errors)
        {
            string[] newDescriptions;

            if (errorDictionary.TryGetValue(error.Code, out string[]? descriptions))
            {
                newDescriptions = new string[descriptions.Length + 1];
                Array.Copy(descriptions, newDescriptions, descriptions.Length);
                newDescriptions[descriptions.Length] = error.Description;
            }
            else
            {
                newDescriptions = [error.Description];
            }

            errorDictionary[error.Code] = newDescriptions;
        }

        return errorDictionary;
    }
}