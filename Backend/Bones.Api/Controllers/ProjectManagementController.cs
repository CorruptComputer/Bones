using Bones.Api.Models;
using Bones.Backend.Features.ProjectManagement.Projects.CreateProject;
using Bones.Backend.Features.ProjectManagement.Projects.GetProjectsByOwner;
using Bones.Database.DbSets.AccountManagement;
using Bones.Shared.Backend.Enums;
using Bones.Shared.Backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace Bones.Api.Controllers;

/// <summary>
///   Handles everything related to Managing Projects
/// </summary>
/// <param name="sender">MediatR sender</param>
public sealed class ProjectManagementController(ISender sender) : BonesControllerBase(sender)
{
    /// <summary>
    ///   Request to create a new project
    /// </summary>
    /// <param name="Name">Name of the project to create</param>
    /// <param name="OrganizationId">Optionally the organization that this should be created under, if not specified will be created for the requesting user.</param>
    public record CreateProjectRequest(string Name, Guid? OrganizationId = null);

    /// <summary>
    ///     Creates a new project
    /// </summary>
    /// <param name="request">The request</param>
    /// <returns>Created if created, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpPost("create", Name = "CreateProjectAsync")]
    [ProducesResponseType<EmptyResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<Dictionary<string, string[]>>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateProjectAsync([FromBody] CreateProjectRequest request)
    {
        CommandResponse response = await Sender.Send(new CreateProjectCommand(request.Name, await GetCurrentBonesUserAsync(), request.OrganizationId));
        if (!response.Success)
        {
            return BadRequest(response.FailureReasons);
        }

        return Ok(EmptyResponse.Value);
    }

    /// <summary>
    ///   Request to get the projects for a given User/Organization
    /// </summary>
    /// <param name="OwnerType">OwnerType to get</param>
    /// <param name="OrganizationId">Optionally the organization that this should be created under, if not specified will be created for the requesting user.</param>
    public record GetProjectsByOwnerRequest(OwnershipType OwnerType, Guid? OrganizationId = null);

    /// <summary>
    ///     Gets the projects for the current user, or specified organization
    /// </summary>
    /// <param name="request">The request</param>
    /// <returns>Created if created, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpGet("projects", Name = "GetProjectsByOwnerAsync")]
    [ProducesResponseType<List<(Guid, string)>>(StatusCodes.Status200OK)]
    [ProducesResponseType<Dictionary<string, string[]>>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> GetProjectsByOwnerAsync([FromBody] GetProjectsByOwnerRequest request)
    {
        BonesUser currentUser = await GetCurrentBonesUserAsync();
        QueryResponse<List<(Guid Id, string Name)>> response = await Sender.Send(new GetProjectsByOwnerQuery(request.OwnerType, request.OrganizationId ?? currentUser.Id, currentUser));
        if (!response.Success)
        {
            return BadRequest(response.FailureReasons);
        }

        return Ok(response.Result);
    }
}