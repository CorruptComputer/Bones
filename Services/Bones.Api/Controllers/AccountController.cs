using Bones.Api.Models.Account;
using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.Audit;
using Bones.Logic.Features.Accounts;
using Bones.Logic.Features.Audits;

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
        List<AccountAudit> audits = (await Sender.Send(new GetMyAccountAudits.Query(user))).Result ?? [];

        return GetMyProfileResponse.FromUser(user, claims: User, audits);
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

    /// <summary>
    ///   Gets or creates a session token for the user and an encryption key to be used for the session.
    ///   All data stored in localStorage on the client side should be encrypted with the encryption key, to protect against XSS attacks.
    ///   
    ///   This encryption key should not be saved in the client, instead save the session token Guid and use it to retrieve the encryption key from the server.
    /// </summary>
    /// <param name="sessionId">The session token to use, if null a new one will be created</param>
    /// <returns></returns>
    [HttpGet("my/session", Name = "GetOrCreateMySessionAsync")]
    [ProducesResponseType<GetOrCreateMySessionResponse>(StatusCodes.Status200OK)]
    public async ValueTask<ActionResult<GetOrCreateMySessionResponse>> GetOrCreateMySessionAsync([FromQuery] Guid? sessionId = null)
    {
        BonesUser user = await GetCurrentBonesUserAsync();

        BonesUserSession? session;
        if (sessionId is null)
        {
            session = await Sender.Send(new CreateMySession.Query(RequestingIpAddress, user));
        }
        else
        {
            session = await Sender.Send(new GetMySession.Query(sessionId.Value, RequestingIpAddress, user));
        }

        if (session is null)
        {
            return NotFound();
        }

        return GetOrCreateMySessionResponse.FromSession(session);
    }
}