using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Bones.Api.Models;
using Bones.Backend.Features.ProjectManagement.Projects.CreateProject;
using Bones.Backend.Features.ProjectManagement.Projects.GetProjectById;
using Bones.Backend.Features.ProjectManagement.Projects.GetProjectsByOwner;
using Bones.Backend.Features.ProjectManagement.Projects.GetProjectsUserCanAccess;
using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.ProjectManagement;
using Bones.Shared.Backend.Enums;
using Bones.Shared.Backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace Bones.Api.Controllers;

/// <summary>
///   Handles everything related to Managing Projects
/// </summary>
/// <param name="sender">MediatR sender</param>
public sealed class ProjectController(ISender sender) : BonesControllerBase(sender)
{
    /// <summary>
    ///   Request to create a new project
    /// </summary>
    /// <param name="Name">Name of the project to create</param>
    /// <param name="OrganizationId">Optionally the organization that this should be created under, if not specified will be created for the requesting user.</param>
    public record CreateProjectRequest([Required] string Name, Guid? OrganizationId = null);

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
    public record GetProjectsByOwnerRequest([Required] OwnershipType OwnerType, Guid? OrganizationId = null);

    /// <summary>
    ///     Gets the projects for the current user, or specified organization
    /// </summary>
    /// <param name="request">The request</param>
    /// <returns>Ok with the results if successful, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpGet("projects/by-owner", Name = "GetProjectsByOwnerAsync")]
    [ProducesResponseType<Dictionary<Guid, string>>(StatusCodes.Status200OK)]
    [ProducesResponseType<Dictionary<string, string[]>>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> GetProjectsByOwnerAsync([FromBody] GetProjectsByOwnerRequest request)
    {
        BonesUser currentUser = await GetCurrentBonesUserAsync();
        QueryResponse<Dictionary<Guid, string>> response = await Sender.Send(new GetProjectsByOwnerQuery(request.OwnerType, request.OrganizationId ?? currentUser.Id, currentUser));
        if (!response.Success)
        {
            return BadRequest(response.FailureReasons);
        }

        return Ok(response.Result);
    }

    /// <summary>
    ///     Gets the projects the current user is able to access
    /// </summary>
    /// <returns>Ok with the results if successful, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpGet("projects", Name = "GetProjectsUserCanAccessAsync")]
    [ProducesResponseType<Dictionary<Guid, string>>(StatusCodes.Status200OK)]
    [ProducesResponseType<Dictionary<string, string[]>>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> GetProjectsUserCanAccessAsync()
    {
        BonesUser currentUser = await GetCurrentBonesUserAsync();
        QueryResponse<Dictionary<Guid, string>> response = await Sender.Send(new GetProjectsUserCanAccessQuery(currentUser));
        if (!response.Success)
        {
            return BadRequest(response.FailureReasons);
        }

        return Ok(response.Result);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ProjectId"></param>
    /// <param name="ProjectName"></param>
    /// <param name="InitiativeCount"></param>
    /// <param name="OwnerType"></param>
    /// <param name="OwnerId"></param>
    [Serializable]
    [JsonSerializable(typeof(GetProjectDashboardResponse))]
    public record GetProjectDashboardResponse(
        Guid ProjectId,
        string ProjectName,
        int InitiativeCount,
        OwnershipType OwnerType,
        Guid OwnerId
    );

    /// <summary>
    ///     Gets a projects dashboard information
    /// </summary>
    /// <param name="projectId"></param>
    /// <returns>Ok with the results if successful, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpGet("{projectId:guid}/dashboard", Name = "GetProjectDashboardAsync")]
    [ProducesResponseType<GetProjectDashboardResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<Dictionary<string, string[]>>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> GetProjectDashboardAsync(Guid projectId)
    {
        BonesUser currentUser = await GetCurrentBonesUserAsync();
        QueryResponse<Project> response = await Sender.Send(new GetProjectByIdQuery(projectId, currentUser));
        if (!response.Success || response.Result == null)
        {
            return BadRequest(response.FailureReasons);
        }

        // We know they won't be null
        Guid ownerId = response.Result.OwnerType == OwnershipType.User
            ? response.Result.OwningUser!.Id
            : response.Result.OwningOrganization!.Id;

        GetProjectDashboardResponse resp = new(response.Result.Id, response.Result.Name,
            response.Result.Initiatives.Count, response.Result.OwnerType, ownerId);

        return Ok(resp);
    }
}