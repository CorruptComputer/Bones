using Bones.Api.Models;
using Bones.Logic.Features.Projects.Initiatives;
using Bones.Logic.Features.Projects.Projects;
using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.ProjectManagement;
using Bones.Shared.Backend.Enums;
using Bones.Shared.Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Bones.Database.DbSets.GenericItems;
using Bones.Api.Models.Project;
using Bones.Logic.Features.GenericItem;

namespace Bones.Api.Controllers;

/// <summary>
///   Handles everything related to Managing Projects
/// </summary>
/// <param name="sender">MediatR sender</param>
public sealed class ProjectController(ISender sender) : BonesControllerBase(sender)
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
    public async ValueTask<ActionResult<IEnumerable<GetProjectQuickSelectResponse>>> GetProjectQuickSelectAsync()
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
    [HttpGet("P{projectId:guid}/dashboard", Name = "GetProjectDashboardAsync")]
    [ProducesResponseType<GetProjectDashboardResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<Dictionary<string, string[]>>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<GetProjectDashboardResponse>> GetProjectDashboardAsync(Guid projectId)
    {
        BonesUser currentUser = await GetCurrentBonesUserAsync();
        QueryResponse<Project> projectResponse = await Sender.Send(new GetProjectByIdQuery(projectId, currentUser));
        QueryResponse<List<Initiative>> initiativesResponse = await Sender.Send(new GetInitiativesByProjectQuery(projectId, currentUser));

        if (!projectResponse.Success || projectResponse.Result is null)
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

        string ownerDisplayName = projectResponse.Result.OwnerType == OwnershipType.User
            // Default it to "Unknown", if someone hasn't set it yet and sees that it'll probably prompt them to add it lol
            ? projectResponse.Result.OwningUser!.DisplayName ?? "Unknown"
            : projectResponse.Result.OwningOrganization!.Name;

        GetProjectDashboardResponse resp = new()
        {
            ProjectId = projectResponse.Result.Id,
            ProjectName = projectResponse.Result.Name,
            OwnerType = projectResponse.Result.OwnerType,
            OwnerId = ownerId,
            OwnerDisplayName = ownerDisplayName,
            InitiativeCount = initiativesResponse.Result.Count,
            Initiatives = initiativesResponse.Result.Select(i =>
                new GetProjectDashboardResponse.InitiativeListModel()
                {
                    InitiativeId = i.Id,
                    InitiativeName = i.Name,
                    QueueCount = i.Queues.Count
                })
        };

        return resp;
    }

    /// <summary>
    ///     Gets a initiatives dashboard information
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="initiativeId"></param>
    /// <returns>Ok with the results if successful, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpGet("P{projectId:guid}/I{initiativeId:guid}/dashboard", Name = "GetInitiativeDashboardAsync")]
    [ProducesResponseType<GetInitiativeDashboardResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<Dictionary<string, string[]>>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<GetInitiativeDashboardResponse>> GetInitiativeDashboardAsync(Guid projectId, Guid initiativeId)
    {
        BonesUser currentUser = await GetCurrentBonesUserAsync();
        QueryResponse<Initiative> initiativeResponse = await Sender.Send(new GetInitiativeByIdQuery(initiativeId, currentUser));

        if (!initiativeResponse.Success || initiativeResponse.Result is null)
        {
            return BadRequest(initiativeResponse.FailureReasons);
        }

        Initiative initiative = initiativeResponse.Result;

        GetInitiativeDashboardResponse resp = new()
        {
            InitiativeId = initiative.Id,
            InitiativeName = initiative.Name,
            ProjectId = initiative.Project.Id,
            WorkItemQueueCount = initiative.Queues.Count,
            WorkItemQueues = initiative.Queues.Select(i =>
                new GetInitiativeDashboardResponse.WorkItemQueueListModel
                {
                    WorkItemQueueId = i.Id,
                    WorkItemQueueName = i.Name,
                    WorkItemCount = i.WorkItems.Count
                })
        };

        return resp;
    }

    /// <summary>
    ///     Gets a projects info by Id
    /// </summary>
    /// <param name="projectId"></param>
    /// <returns>Ok with the results if successful, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpGet("P{projectId:guid}/settings", Name = "GetProjectSettingsAsync")]
    [ProducesResponseType<GetProjectSettingsResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<Dictionary<string, string[]>>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<GetProjectSettingsResponse>> GetProjectSettingsAsync(Guid projectId)
    {
        BonesUser currentUser = await GetCurrentBonesUserAsync();
        QueryResponse<Project> projectResponse = await Sender.Send(new GetProjectByIdQuery(projectId, currentUser));

        if (!projectResponse.Success || projectResponse.Result is null)
        {
            return BadRequest(projectResponse.FailureReasons);
        }

        QueryResponse<List<GenericItemField>> itemFields = await Sender.Send(new GetItemFieldsByProjectQuery(projectId, currentUser));
        QueryResponse<List<GenericItemLayout>> itemLayouts = await Sender.Send(new GetItemLayoutsByProjectQuery(projectId, currentUser));

        if (!itemFields.Success || itemFields.Result is null)
        {
            return BadRequest(itemFields.FailureReasons);
        }

        if (!itemLayouts.Success || itemLayouts.Result is null)
        {
            return BadRequest(itemLayouts.FailureReasons);
        }

        GetProjectSettingsResponse resp = GetProjectSettingsResponse.FromInternal(projectResponse.Result, itemFields.Result, itemLayouts.Result);

        return resp;
    }

    /// <summary>
    ///     Gets the Item Fields available in a project
    /// </summary>
    /// <param name="projectId">The ID of the project</param>
    /// <returns>The item fields in the project.</returns>
    [HttpGet("P{projectId:guid}/fields", Name = "GetProjectItemFieldsAsync")]
    [ProducesResponseType<GetProjectItemFieldsResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<GetProjectItemFieldsResponse>> GetProjectItemFieldsAsync(Guid projectId)
    {
        QueryResponse<List<GenericItemField>> fieldResponse = await Sender.Send(new GetItemFieldsByProjectQuery(projectId, await GetCurrentBonesUserAsync()));

        if (!fieldResponse.Success || fieldResponse.Result is null)
        {
            return BadRequest(ErrorResponse.FromQueryResponse(fieldResponse));
        }

        return GetProjectItemFieldsResponse.FromInternalList(fieldResponse.Result);
    }

    /// <summary>
    ///     Gets the latest version of a field
    /// </summary>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="fieldId">The ID of the field</param>
    /// <returns>The latest version of the requested field.</returns>
    [HttpGet("P{projectId:guid}/fields/F{fieldId:guid}/latest", Name = "GetLatestItemFieldVersionAsync")]
    [ProducesResponseType<GetLatestItemFieldVersionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<GetLatestItemFieldVersionResponse>> GetLatestItemFieldVersionAsync(Guid projectId, Guid fieldId)
    {
        QueryResponse<GenericItemField?> fieldResponse = await Sender.Send(new GetItemFieldByIdQuery(fieldId, await GetCurrentBonesUserAsync()));

        if (!fieldResponse.Success || fieldResponse.Result is null)
        {
            return BadRequest(ErrorResponse.FromQueryResponse(fieldResponse));
        }

        return GetLatestItemFieldVersionResponse.FromInternal(fieldResponse.Result);
    }

    /// <summary>
    ///     Gets the latest version of a layout
    /// </summary>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="layoutId">The ID of the layout</param>
    /// <returns>The latest version of the requested layout.</returns>
    [HttpGet("P{projectId:guid}/layouts/L{layoutId:guid}/latest", Name = "GetLatestItemLayoutVersionAsync")]
    [ProducesResponseType<GetLatestItemLayoutVersionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<GetLatestItemLayoutVersionResponse>> GetLatestItemLayoutVersionAsync(Guid projectId, Guid layoutId)
    {
        QueryResponse<GenericItemLayout?> layoutResponse = await Sender.Send(new GetItemLayoutById.Query(layoutId, await GetCurrentBonesUserAsync()));

        if (!layoutResponse.Success || layoutResponse.Result is null)
        {
            return BadRequest(ErrorResponse.FromQueryResponse(layoutResponse));
        }

        return GetLatestItemLayoutVersionResponse.FromInternal(layoutResponse.Result);
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
    [HttpPost("P{projectId:guid}/initiative/create", Name = "CreateInitiativeAsync")]
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

    /// <summary>
    ///     Creates a new item field in a project
    /// </summary>
    /// <param name="projectId">The ID of the project to create this in</param>
    /// <param name="request">The request</param>
    /// <returns>Created if created, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpPost("P{projectId:guid}/fields/create", Name = "CreateItemFieldAsync")]
    [ProducesResponseType<Guid>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<Guid>> CreateItemFieldAsync(Guid projectId, [FromBody] CreateItemFieldRequest request)
    {
        CommandResponse response = await Sender.Send(request.ToInternal(projectId, await GetCurrentBonesUserAsync()));
        if (!response.Success)
        {
            return BadRequest(ErrorResponse.FromCommandResponse(response));
        }

        return response.Id ?? Guid.Empty;
    }

    /// <summary>
    ///     Creates a new item field version in a project
    /// </summary>
    /// <param name="projectId">The ID of the project to create this in</param>
    /// <param name="fieldId">The ID of the field to add this version to</param>
    /// <param name="request">The request</param>
    /// <returns>Created if created, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpPost("P{projectId:guid}/fields/F{fieldId:guid}", Name = "CreateItemFieldVersionAsync")]
    [ProducesResponseType<Guid>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<Guid>> CreateItemFieldVersionAsync(Guid projectId, Guid fieldId, [FromBody] CreateItemFieldVersionRequest request)
    {
        CommandResponse response = await Sender.Send(request.ToInternal(fieldId, await GetCurrentBonesUserAsync()));
        if (!response.Success)
        {
            return BadRequest(ErrorResponse.FromCommandResponse(response));
        }

        return response.Id ?? Guid.Empty;
    }

    /// <summary>
    ///     Creates a new item layout in a project
    /// </summary>
    /// <param name="projectId">The ID of the project to create this in</param>
    /// <param name="request">The request</param>
    /// <returns>Created if created, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpPost("P{projectId:guid}/layouts/create", Name = "CreateItemLayoutAsync")]
    [ProducesResponseType<Guid>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<Guid>> CreateItemLayoutAsync(Guid projectId, [FromBody] CreateItemLayoutRequest request)
    {
        CommandResponse response = await Sender.Send(request.ToInternal(projectId, await GetCurrentBonesUserAsync()));
        if (!response.Success)
        {
            return BadRequest(ErrorResponse.FromCommandResponse(response));
        }

        return response.Id ?? Guid.Empty;
    }

    /// <summary>
    ///     Creates a new item layout version in a project
    /// </summary>
    /// <param name="projectId">The ID of the project to create this in</param>
    /// <param name="layoutId">The ID of the layout to add this version to</param>
    /// <param name="request">The request</param>
    /// <returns>Created if created, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpPost("P{projectId:guid}/layouts/L{layoutId:guid}", Name = "CreateItemLayoutVersionAsync")]
    [ProducesResponseType<Guid>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<Guid>> CreateItemLayoutVersionAsync(Guid projectId, Guid layoutId, [FromBody] CreateItemLayoutVersionRequest request)
    {
        CommandResponse response = await Sender.Send(request.ToInternal(layoutId, await GetCurrentBonesUserAsync()));
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