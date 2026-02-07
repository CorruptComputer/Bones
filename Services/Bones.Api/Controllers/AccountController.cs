using Bones.Api.Controllers.Base;
using Bones.Api.Models.Accounts;
using Bones.Database.DbSets.Accounts;
using Bones.Database.Operations.Accounts;

namespace Bones.Api.Controllers;

/// <summary>
///   Handles everything related to Accounts other than your own
/// </summary>
public sealed class AccountController(ISender sender) : AuthenticatedControllerBase(sender)
{
    /// <summary>
    ///   Returns a users public profile
    /// </summary>
    /// <returns><see cref="GetPublicProfileResponse"/></returns>
    [HttpGet("{bonesUserId:Guid}/public-profile", Name = "GetPublicProfileAsync")]
    [ProducesResponseType<GetPublicProfileResponse>(StatusCodes.Status200OK)]
    public async ValueTask<ActionResult<GetPublicProfileResponse>> GetPublicProfileAsync(Guid bonesUserId)
    {
        BonesUser? user = await Sender.Send(new GetUserByIdDb.Query(bonesUserId));

        if (user is null)
        {
            return NotFound(new ErrorResponse("User not found"));
        }

        return GetPublicProfileResponse.FromUser(user);
    }
}