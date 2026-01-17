using Bones.Logic.Features.Initiatives;
using Bones.Logic.Features.Projects;
using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.ProjectManagement;
using Bones.Database.DbSets.Items;
using Bones.Api.Models.Project;
using Bones.Logic.Features.Items;
using Bones.Api.Controllers.Base;
using Bones.Shared.Backend.Enums;

namespace Bones.Api.Controllers;

/// <summary>
///   Handles everything related to Managing Projects
/// </summary>
/// <param name="sender">Questy sender</param>
public sealed class ProjectController(ISender sender) : AuthenticatedControllerBase(sender)
{
    #region GET
    /// <summary>
    ///   Gets a projects dashboard information
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
        QueryResponse<List<ItemLayout>> layoutsResponse = await Sender.Send(new GetItemLayoutsByProject.Query(projectId, currentUser));

        if (!projectResponse.Success || projectResponse.Result is null)
        {
            return BadRequest(projectResponse.FailureReasons);
        }

        if (!initiativesResponse.Success || initiativesResponse.Result is null)
        {
            return BadRequest(initiativesResponse.FailureReasons);
        }

        if (!layoutsResponse.Success || layoutsResponse.Result is null)
        {
            return BadRequest(layoutsResponse.FailureReasons);
        }

        GetProjectDashboardResponse resp = new()
        {
            ProjectId = projectResponse.Result.Id,
            ProjectName = projectResponse.Result.Name,
            InitiativeCount = initiativesResponse.Result.Count,
            AssetTypes = layoutsResponse.Result.Where(layout => layout.Current?.LayoutUse == ItemLayoutUse.Assets).Select(layout =>
                new GetProjectDashboardResponse.AssetTypesListModel
                {
                    LayoutId = layout.Id,
                    LayoutName = layout.Current!.Name,
                }),
            Initiatives = initiativesResponse.Result.Select(i =>
                new GetProjectDashboardResponse.InitiativeListModel
                {
                    InitiativeId = i.Id,
                    InitiativeName = i.Name,
                    QueueCount = i.Queues.Count
                })
        };

        return resp;
    }

    /// <summary>
    ///   Gets a projects info by Id
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

        QueryResponse<List<ItemField>> itemFields = await Sender.Send(new GetItemFieldsByProject.Query(projectId, currentUser));
        QueryResponse<List<ItemLayout>> itemLayouts = await Sender.Send(new GetItemLayoutsByProject.Query(projectId, currentUser));

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
    ///   Gets the Item Fields available in a project
    /// </summary>
    /// <param name="projectId">The ID of the project</param>
    /// <returns>The item fields in the project.</returns>
    [HttpGet("{projectId:guid}/fields", Name = "GetProjectItemFieldsAsync")]
    [ProducesResponseType<GetProjectItemFieldsResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<GetProjectItemFieldsResponse>> GetProjectItemFieldsAsync(Guid projectId)
    {
        QueryResponse<List<ItemField>> fieldResponse = await Sender.Send(new GetItemFieldsByProject.Query(projectId, await GetCurrentBonesUserAsync()));

        if (!fieldResponse.Success || fieldResponse.Result is null)
        {
            return BadRequest(ErrorResponse.FromQueryResponse(fieldResponse));
        }

        return GetProjectItemFieldsResponse.FromInternalList(fieldResponse.Result);
    }

    /// <summary>
    ///   Gets the item layouts for a project, optionally filtered by the uses they are enabled for
    /// </summary>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="layoutUse"></param>
    /// <returns>The latest version of the requested layout.</returns>
    [HttpGet("{projectId:guid}/layouts", Name = "GetProjectLayoutsAsync")]
    [ProducesResponseType<List<GetProjectLayoutsResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<List<GetProjectLayoutsResponse>>> GetProjectLayoutsAsync(Guid projectId, [FromQuery] ItemLayoutUse? layoutUse = null)
    {
        QueryResponse<List<ItemLayout>> layouts = await Sender.Send(new GetItemLayoutsByProject.Query(projectId, await GetCurrentBonesUserAsync()));

        if (!layouts.Success || layouts.Result is null)
        {
            return BadRequest(ErrorResponse.FromQueryResponse(layouts));
        }

        return GetProjectLayoutsResponse.FromInternalList(layouts.Result, layoutUse);
    }

    /// <summary>
    ///   Gets the initiatives in a project
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
    ///   Creates a new project
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

        return response.Ids[nameof(Project)];
    }

    /// <summary>
    ///   Creates a new project
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

        return response.Ids[nameof(Initiative)];
    }
    #endregion
}