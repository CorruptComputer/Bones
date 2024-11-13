using Bones.Database.DbSets.AccountManagement;
using Bones.Shared.Consts;
using Microsoft.AspNetCore.Mvc;

namespace Bones.Api.Controllers;

/// <summary>
///   Handles everything related to User Accounts
/// </summary>
/// <param name="sender">MediatR sender</param>
/// <remarks>
///   Created using this as a reference:
///   https://github.com/dotnet/aspnetcore/blob/main/src/Identity/Core/src/IdentityApiEndpointRouteBuilderExtensions.cs
/// </remarks>
public sealed partial class AccountController(ISender sender) : BonesControllerBase(sender)
{
    /// <summary>
    ///   Returns a users own full profile info
    /// </summary>
    /// <returns><see cref="GetMyProfileResponse"/></returns>
    [HttpGet("my/profile", Name = "GetMyProfileAsync")]
    [ProducesResponseType<GetMyProfileResponse>(StatusCodes.Status200OK)]
    public async ValueTask<ActionResult<GetMyProfileResponse>> GetMyProfileAsync()
    {
        BonesUser user = await GetCurrentBonesUserAsync();

        return new GetMyProfileResponse(
            user.Email ?? string.Empty,
            user.EmailConfirmed,
            user.EmailConfirmedDateTime,
            user.DisplayName ?? user.Email ?? string.Empty,
            user.CreateDateTime,
            IsSysAdmin: User.IsInRole(SystemRoles.SYSTEM_ADMINISTRATORS)
        );
    }
}