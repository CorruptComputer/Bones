using Bones.Backend.Features.ProjectManagement.Projects;
using Bones.Database.DbSets.AccountManagement;
using Bones.Shared.Backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace Bones.Api.Controllers;

public sealed class ProjectManagementController(BonesUser user, ISender sender) : BonesControllerBase(sender)
{
    public record CreateProjectAsyncRequest(string Name);
    /// <summary>
    ///     Creates a new project
    /// </summary>
    /// <param name="request">The name for the project</param>
    /// <returns>Created if created, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpPost("create", Name = "CreateProjectAsync")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateProjectAsync([FromBody] CreateProjectAsyncRequest request)
    {
        CommandResponse response = await Sender.Send(new CreateProject.Command(request.Name, user));
        if (!response.Success)
        {
            return BadRequest(response.FailureReason);
        }

        return Created();
    }
}