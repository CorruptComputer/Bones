using Bones.Database.DbSets.AccountManagement;
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
public sealed class AccountController(ISender sender) : BonesControllerBase(sender)
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Email"></param>
    /// <param name="DisplayName"></param>
    public record GetMyBasicInfoResponse(string Email, string DisplayName);

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet("my/basic-info", Name = "GetMyBasicInfoAsync")]
    [ProducesResponseType<GetMyBasicInfoResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetMyBasicInfoAsync()
    {
        BonesUser user = await GetCurrentBonesUserAsync();

        return Ok(new GetMyBasicInfoResponse(
            user.Email ?? string.Empty,
            user.DisplayName ?? user.Email ?? string.Empty));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Email"></param>
    /// <param name="EmailConfirmed"></param>
    /// <param name="EmailConfirmedDateTime"></param>
    /// <param name="DisplayName"></param>
    /// <param name="CreateDateTime"></param>
    public record GetMyProfileResponse(string Email, bool EmailConfirmed, DateTimeOffset? EmailConfirmedDateTime, string DisplayName, DateTimeOffset CreateDateTime);

    /// <summary>
    ///   Returns a users own full profile info
    /// </summary>
    /// <returns><see cref="GetMyProfileResponse"/></returns>
    [HttpGet("my/profile", Name = "GetMyProfileAsync")]
    [ProducesResponseType<GetMyProfileResponse>(StatusCodes.Status200OK)]
    public async ValueTask<ActionResult> GetMyProfileAsync()
    {
        BonesUser user = await GetCurrentBonesUserAsync();


        return Ok(new GetMyProfileResponse(
            user.Email ?? string.Empty,
            user.EmailConfirmed,
            user.EmailConfirmedDateTime,
            user.DisplayName ?? user.Email ?? string.Empty,
            user.CreateDateTime
        ));
    }
}