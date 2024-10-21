using System.Net.Mime;
using Bones.Api.Models;
using Bones.Backend.Features.AccountManagement.GetUserByClaimsPrincipal;
using Bones.Database.DbSets.AccountManagement;
using Bones.Shared.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
[ProducesResponseType<EmptyResponse>(StatusCodes.Status401Unauthorized)]
[ProducesResponseType<EmptyResponse>(StatusCodes.Status403Forbidden)]
[ProducesResponseType<EmptyResponse>(StatusCodes.Status500InternalServerError)]
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

    /// <summary>
    ///   Gets the errors from an IdentityResult in a format that we can return.
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    protected static Dictionary<string, string[]> ReadErrorsFromIdentityResult(IdentityResult result)
    {
        Dictionary<string, string[]> errorDictionary = new();

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