using Bones.Api.Models;
using Bones.Backend.Features.ProjectManagement.Projects;
using Bones.Database.DbSets.AccountManagement;
using Bones.Shared.Backend.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Bones.Api.Controllers;

/// <summary>
/// 
/// </summary>
/// <param name="user"></param>
/// <param name="sender"></param>
public sealed class ProjectManagementController(BonesUser user, ISender sender) : BonesControllerBase(sender)
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Name"></param>
    public record CreateProjectAsyncRequest(string Name);

    /// <summary>
    ///     Creates a new project
    /// </summary>
    /// <param name="request">The name for the project</param>
    /// <returns>Created if created, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpPost("create", Name = "CreateProjectAsync")]
    [ProducesResponseType<ActionResult<string>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ActionResult<string>>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateProjectAsync([FromBody] CreateProjectAsyncRequest request)
    {
        CommandResponse response = await Sender.Send(new CreateProject.Command(request.Name, user));
        if (!response.Success)
        {
            return BadRequest(response.FailureReason);
        }

        return Ok(string.Empty);
    }
}