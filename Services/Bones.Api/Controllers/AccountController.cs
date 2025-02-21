using Bones.Api.Models.Account;
using Bones.Database.DbSets.AccountManagement;

namespace Bones.Api.Controllers;

/// <summary>
///   Handles everything related to User Accounts
/// </summary>
/// <param name="sender">MediatR sender</param>
/// <remarks>
///   Created using this as a reference:
///   https://github.com/dotnet/aspnetcore/blob/main/src/Identity/Core/src/IdentityApiEndpointRouteBuilderExtensions.cs
/// </remarks>
public sealed class AccountController(ISender sender) : BonesControllerBase(sender)
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

        return GetMyProfileResponse.FromUser(user, User);
    }

    /// <summary>
    ///     Updates the current users profile
    /// </summary>
    /// <param name="request">The request</param>
    /// <returns>true if successful, what went wrong otherwise</returns>
    [HttpPut("my/profile", Name = "UpdateMyProfileAsync")]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<bool>> UpdateMyProfileAsync([FromBody] UpdateMyProfileRequest request)
    {
        CommandResponse response = await Sender.Send(request.ToInternal(await GetCurrentBonesUserAsync()));

        if (!response.Success)
        {
            return BadRequest(ErrorResponse.FromCommandResponse(response));
        }

        return response.Success;
    }
}