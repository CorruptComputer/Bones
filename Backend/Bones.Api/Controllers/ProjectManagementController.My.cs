using Bones.Api.Models;
using Bones.Backend.Features.ProjectManagement.Projects.CreateProject;
using Bones.Shared.Backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace Bones.Api.Controllers;

public sealed partial class ProjectManagementController
{
    /// <summary>
    ///   Request to create a new project for the current user
    /// </summary>
    /// <param name="Name">Name of the project to create</param>
    public record CreateMyProjectRequest(string Name);

    /// <summary>
    ///     Creates a new project for the current user
    /// </summary>
    /// <param name="request">The name for the project</param>
    /// <returns>Created if created, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpPost("my/create", Name = "CreateMyProjectAsync")]
    [ProducesResponseType<ActionResult<EmptyResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ActionResult<Dictionary<string, string[]>>>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateMyProjectAsync([FromBody] CreateMyProjectRequest request)
    {
        CommandResponse response = await Sender.Send(new CreateProjectCommand(request.Name, await GetCurrentBonesUserAsync()));
        if (!response.Success)
        {
            return BadRequest(response.FailureReasons);
        }

        return Ok(string.Empty);
    }
}