using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;

namespace Bones.Api.Controllers;

/// <summary>
///     Base controller for the Bones API, everything should extend from this.
/// </summary>
[ApiController]
[Route("[controller]")]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
public class BonesControllerBase(ISender sender) : ControllerBase
{
    /// <summary>
    ///     MediatR sender for commands and queries to the backend, you can technically send things straight to the DB here,
    ///     but don't do that.
    /// </summary>
    protected ISender Sender => sender;
}