using System.Net;
using System.Net.Mime;

namespace Bones.Api.Controllers.Base;

/// <summary>
///   Base controller for the Bones API, everything should extend from this.
/// </summary>
/// <param name="sender">MediatR sender</param>
[ApiController]
[Route("[controller]")]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType<ErrorResponse>(StatusCodes.Status500InternalServerError)]
public abstract class BonesControllerBase(ISender sender) : ControllerBase
{
    /// <summary>
    ///   MediatR sender for commands and queries to the Logic layer,
    ///   you can technically send things straight to the DB here, but don't do that.
    /// </summary>
    protected ISender Sender => sender;

    /// <summary>
    ///   Gets the IP address of the client making the request, or IPAddress.None if it can't be determined.
    /// </summary>
    protected IPAddress RequestingIpAddress => Request.HttpContext.Connection.RemoteIpAddress ?? IPAddress.None;
}