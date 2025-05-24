using Bones.Logic.Features.Accounts;
using Bones.Database.DbSets.AccountManagement;
using Bones.Shared.Exceptions;
using Microsoft.AspNetCore.Authorization;

namespace Bones.Api.Controllers.Base;

/// <summary>
///   Base for authenticated controllers.
/// </summary>
/// <param name="sender">MediatR sender</param>
[Authorize]
[ProducesResponseType<UnauthorizedResult>(StatusCodes.Status401Unauthorized)] // Returned by the [Authorize] attribute
[ProducesResponseType<ErrorResponse>(StatusCodes.Status401Unauthorized)] // Returned by UnauthenticatedException
[ProducesResponseType<ErrorResponse>(StatusCodes.Status403Forbidden)] // Returned by ForbiddenException
[ProducesResponseType<ErrorResponse>(StatusCodes.Status500InternalServerError)] // Returned by any other exception
public abstract class AuthenticatedControllerBase(ISender sender) : BonesControllerBase(sender)
{
    /// <summary>
    ///   Gets the user for the current request
    /// </summary>
    /// <returns></returns>
    protected async Task<BonesUser> GetCurrentBonesUserAsync()
    {
        BonesUser? user = await Sender.Send(new GetUserByClaimsPrincipal.Query(User));

        return user ?? throw new UnauthenticatedException();
    }
}