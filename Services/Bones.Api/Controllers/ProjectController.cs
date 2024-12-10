using Bones.Api.Models;
using Bones.Logic.Features.Projects.Initiatives;
using Bones.Logic.Features.Projects.Projects.CreateProject;
using Bones.Logic.Features.Projects.Projects.GetProjectById;
using Bones.Logic.Features.Projects.Projects.GetProjectsByOwner;
using Bones.Logic.Features.Projects.Projects.GetProjectsUserCanAccess;
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
public sealed partial class ProjectController(ISender sender) : BonesControllerBase(sender)
{
    #region GET
    /// <summary>
    ///     Gets the projects for the current user, or specified organization
    /// </summary>
    /// <param name="request">The request</param>
    /// <returns>Ok with the results if successful, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpGet("projects/by-owner", Name = "GetProjectsByOwnerAsync")]
    [ProducesResponseType<Dictionary<Guid, string>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<Dictionary<string, string[]>>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<Dictionary<Guid, string>>> GetProjectsByOwnerAsync([FromBody] GetProjectsByOwnerRequest request)
    {
        BonesUser currentUser = await GetCurrentBonesUserAsync();
        QueryResponse<Dictionary<Guid, string>> response = await Sender.Send(new GetProjectsByOwnerQuery(request.OwnerType, request.OrganizationId ?? currentUser.Id, currentUser));
        if (!response.Success)
        {
            return BadRequest(response.FailureReasons);
        }

        if (response.Result == null || response.Result.Count == 0)
        {
            return NoContent();
        }

        return response.Result;
    }

    /// <summary>
    ///     Gets the users current quick select projects
    /// </summary>
    /// <returns>Ok with the results if successful, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpGet("projects/quick-select", Name = "GetProjectQuickSelectAsync")]
    [ProducesResponseType<List<GetProjectQuickSelectResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<EmptyResponse>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<Dictionary<string, string[]>>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<List<GetProjectQuickSelectResponse>>> GetProjectQuickSelectAsync()
    {
        BonesUser currentUser = await GetCurrentBonesUserAsync();

        // TODO: Make this customizable 
        QueryResponse<Dictionary<Guid, string>> response = await Sender.Send(new GetProjectsUserCanAccessQuery(currentUser));
        if (!response.Success)
        {
            return BadRequest(response.FailureReasons);
        }

        if (response.Result is null)
        {
            return NotFound(EmptyResponse.Value);
        }

        return response.Result.Select(kvp => 
            new GetProjectQuickSelectResponse
            {
                ProjectId = kvp.Key,
                ProjectName = kvp.Value
            }).ToList();
    }

    /// <summary>
    ///     Gets a projects dashboard information
    /// </summary>
    /// <param name="projectId"></param>
    /// <returns>Ok with the results if successful, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpGet("{projectId:guid}/dashboard", Name = "GetProjectDashboardAsync")]
    [ProducesResponseType<GetProjectDashboardResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<Dictionary<string, string[]>>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<GetProjectDashboardResponse>> GetProjectDashboardAsync(Guid projectId)
    {
        BonesUser currentUser = await GetCurrentBonesUserAsync();
        QueryResponse<Project> projectResponse = await Sender.Send(new GetProjectByIdQuery(projectId, currentUser));
        QueryResponse<List<Initiative>> initiativesResponse = await Sender.Send(new GetInitiativesByProjectQuery(projectId, currentUser));

        if (!projectResponse.Success || projectResponse.Result is null || !initiativesResponse.Success || initiativesResponse.Result is null)
        {
            return BadRequest(projectResponse.FailureReasons);
        }

        if (!initiativesResponse.Success || initiativesResponse.Result is null)
        {
            return BadRequest(initiativesResponse.FailureReasons);
        }

        // We know they won't be null
        Guid ownerId = projectResponse.Result.OwnerType == OwnershipType.User
            ? projectResponse.Result.OwningUser!.Id
            : projectResponse.Result.OwningOrganization!.Id;

        GetProjectDashboardResponse resp = new()
        {
            ProjectId = projectResponse.Result.Id,
            ProjectName = projectResponse.Result.Name,
            OwnerType = projectResponse.Result.OwnerType,
            OwnerId = ownerId,
            InitiativeCount = initiativesResponse.Result.Count,
            Initiatives = initiativesResponse.Result.Select(i => 
                new InitiativeListModel
                {
                    InitiativeId = i.Id,
                    InitiativeName = i.Name,
                    QueueCount = i.Queues.Count
                }).ToList()
        };

        return resp;
    }
    #endregion

    #region POST
    /// <summary>
    ///     Creates a new project
    /// </summary>
    /// <param name="request">The request</param>
    /// <returns>Created if created, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpPost("create", Name = "CreateProjectAsync")]
    [ProducesResponseType<Guid>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<Guid>> CreateProjectAsync([FromBody] CreateProjectRequest request)
    {
        CommandResponse response = await Sender.Send(new CreateProjectCommand(request.Name, await GetCurrentBonesUserAsync(), request.OrganizationId));
        if (!response.Success)
        {
            return BadRequest(ErrorResponse.FromCommandResponse(response));
        }

        return response.Id ?? Guid.Empty;
    }

    /// <summary>
    ///     Creates a new project
    /// </summary>
    /// <param name="projectId">The ID of the project to create this in</param>
    /// <param name="request">The request</param>
    /// <returns>Created if created, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpPost("{projectId:guid}/initiative/create", Name = "CreateInitiativeAsync")]
    [ProducesResponseType<Guid>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<Guid>> CreateInitiativeAsync(Guid projectId, [FromBody] CreateInitiativeRequest request)
    {
        CommandResponse response = await Sender.Send(new CreateInitiativeCommand(request.Name, projectId, await GetCurrentBonesUserAsync()));
        if (!response.Success)
        {
            return BadRequest(ErrorResponse.FromCommandResponse(response));
        }

        return response.Id ?? Guid.Empty;
    }
    #endregion

    #region PUT

    #endregion

    #region DELETE

    #endregion
}