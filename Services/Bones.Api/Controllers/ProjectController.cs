using Bones.Logic.Features.Projects.Initiatives;
using Bones.Logic.Features.Projects.Projects;
using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.ProjectManagement;
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
        QueryResponse<Dictionary<Guid, string>> response = await Sender.Send(new GetProjectsByOwner.Query(request.OwnerType, request.OrganizationId ?? currentUser.Id, currentUser));
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
        QueryResponse<Dictionary<Guid, string>> response = await Sender.Send(new GetProjectsUserCanAccess.Query(currentUser));
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
        QueryResponse<Project> projectResponse = await Sender.Send(new GetProjectById.Query(projectId, currentUser));
        QueryResponse<List<Initiative>> initiativesResponse = await Sender.Send(new GetInitiativesByProject.Query(projectId, currentUser));

        if (!projectResponse.Success || projectResponse.Result is null)
        {
            return BadRequest(projectResponse.FailureReasons);
        }

        if (!initiativesResponse.Success || initiativesResponse.Result is null)
        {
            return BadRequest(initiativesResponse.FailureReasons);
        }

        GetProjectDashboardResponse resp = new()
        {
            ProjectId = projectResponse.Result.Id,
            ProjectName = projectResponse.Result.Name,
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
    ///     Gets a projects info by Id
    /// </summary>
    /// <param name="projectId"></param>
    /// <returns>Ok with the results if successful, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpGet("{projectId:guid}/settings", Name = "GetProjectSettingsAsync")]
    [ProducesResponseType<GetProjectSettingsResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<Dictionary<string, string[]>>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<GetProjectSettingsResponse>> GetProjectSettingsAsync(Guid projectId)
    {
        BonesUser currentUser = await GetCurrentBonesUserAsync();
        QueryResponse<Project> projectResponse = await Sender.Send(new GetProjectById.Query(projectId, currentUser));

        if (!projectResponse.Success || projectResponse.Result is null)
        {
            return BadRequest(projectResponse.FailureReasons);
        }

        QueryResponse<List<GenericItemField>> itemFields = await Sender.Send(new GetItemFieldsByProject.Query(projectId, currentUser));
        QueryResponse<List<GenericItemLayout>> itemLayouts = await Sender.Send(new GetItemLayoutsByProject.Query(projectId, currentUser));

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
    [HttpGet("{projectId:guid}/fields", Name = "GetProjectItemFieldsAsync")]
    [ProducesResponseType<GetProjectItemFieldsResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<GetProjectItemFieldsResponse>> GetProjectItemFieldsAsync(Guid projectId)
    {
        QueryResponse<List<GenericItemField>> fieldResponse = await Sender.Send(new GetItemFieldsByProject.Query(projectId, await GetCurrentBonesUserAsync()));

        if (!fieldResponse.Success || fieldResponse.Result is null)
        {
            return BadRequest(ErrorResponse.FromQueryResponse(fieldResponse));
        }

        return GetProjectItemFieldsResponse.FromInternalList(fieldResponse.Result);
    }

    /// <summary>
    ///     
    /// </summary>
    /// <param name="projectId">The ID of the project</param>
    /// <returns>The initiatives in the project.</returns>
    [HttpGet("{projectId:guid}/initiatives", Name = "GetInitiativesInProjectAsync")]
    [ProducesResponseType<List<GetInitiativesInProjectResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<List<GetInitiativesInProjectResponse>>> GetInitiativesInProjectAsync(Guid projectId)
    {
        QueryResponse<List<Initiative>> initiativeResponse = await Sender.Send(new GetInitiativesByProject.Query(projectId, await GetCurrentBonesUserAsync()));

        if (!initiativeResponse.Success || initiativeResponse.Result is null)
        {
            return BadRequest(ErrorResponse.FromQueryResponse(initiativeResponse));
        }

        return GetInitiativesInProjectResponse.FromInternalList(initiativeResponse.Result);
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
        CommandResponse response = request.Preset.HasValue
            ? await Sender.Send(new CreateProjectWithPreset.Command(request.Name, request.Preset.Value, await GetCurrentBonesUserAsync(), request.OrganizationId))
            : await Sender.Send(new CreateProject.Command(request.Name, await GetCurrentBonesUserAsync(), request.OrganizationId));

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
        CommandResponse response = await Sender.Send(new CreateInitiative.Command(request.Name, projectId, await GetCurrentBonesUserAsync()));
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
    [HttpPost("{projectId:guid}/fields/create", Name = "CreateItemFieldAsync")]
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
    [HttpPost("{projectId:guid}/fields/{fieldId:guid}", Name = "CreateItemFieldVersionAsync")]
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
    [HttpPost("{projectId:guid}/layouts/create", Name = "CreateItemLayoutAsync")]
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
    [HttpPost("{projectId:guid}/layouts/{layoutId:guid}", Name = "CreateItemLayoutVersionAsync")]
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
}