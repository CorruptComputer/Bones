using Bones.Database.DbSets.AccountManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bones.Api.Controllers;

public sealed class AccountManagementController(ISender sender) : BonesControllerBase(sender)
{
    [HttpPost("logout", Name = "LogoutAsync")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [AllowAnonymous]
    public ActionResult LogoutAsync()
    {
        Response.Cookies.Append(".AspNetCore.Identity.Application", string.Empty, new()
        {
            Secure = true,
            HttpOnly = true,
            Expires = DateTimeOffset.Now.AddDays(-1)
        });

        return Ok();
    }
}