using Bones.Shared.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bones.Api.Controllers;

/// <summary>
///   Handles SysAdmin stuffs
/// </summary>
[Authorize(Roles = SystemRoles.SYSTEM_ADMINISTRATORS)]
public class SysAdminController(ISender sender) : BonesControllerBase(sender)
{
    /// <summary>
    ///   Ping, pong!
    /// </summary>
    /// <returns>A super secret passphrase that should absolutely never be shared under any circumstances.</returns>
    [HttpPost("ping", Name = "PingAsync")]
    [ProducesResponseType<string>(StatusCodes.Status200OK)]
    public ActionResult<string> PingAsync()
    {
        return "pong";
    }
}