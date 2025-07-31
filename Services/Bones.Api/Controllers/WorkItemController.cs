using Bones.Api.Controllers.Base;
using Bones.Api.Models.WorkItems;
using Bones.Api.Models.WorkItems.Actions;
using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.WorkItemManagement;
using Bones.Logic.Features.WorkItems.WorkItems;

namespace Bones.Api.Controllers;

/// <summary>
///   Handles work items
/// </summary>
/// <param name="sender"></param>
public class WorkItemController(ISender sender) : AuthenticatedControllerBase(sender)
{
    #region GET
    /// <summary>
    ///   Gets a work item by its ID
    /// </summary>
    /// <param name="workItemId"></param>
    /// <returns>Ok with the results if successful, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpGet("{workItemId:guid}", Name = "GetWorkItemByIdAsync")]
    [ProducesResponseType<GetWorkItemByIdResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async ValueTask<ActionResult<GetWorkItemByIdResponse>> GetWorkItemByIdAsync(Guid workItemId)
    {
        WorkItem? item = await Sender.Send(new GetWorkItemById.Query(workItemId, await GetCurrentBonesUserAsync()));

        if (item is null)
        {
            return NotFound(new ErrorResponse());
        }

        return GetWorkItemByIdResponse.FromInternal(item);
    }
    #endregion

    #region POST
    /// <summary>
    ///   Assigns a work item
    /// </summary>
    /// <param name="request"></param>
    /// <returns>Ok with the results if successful, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpPost("action/assign", Name = "AssignWorkItemActionAsync")]
    [ProducesResponseType<WorkItemActionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<WorkItemActionResponse>> AssignWorkItemActionAsync([FromBody] AssignWorkItemAction request)
    {
        return await PerformWorkItemActionAsync(request);
    }

    /// <summary>
    ///   Creates a new work item
    /// </summary>
    /// <param name="request"></param>
    /// <returns>Ok with the results if successful, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpPost("action/create", Name = "CreateWorkItemActionAsync")]
    [ProducesResponseType<WorkItemActionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<WorkItemActionResponse>> CreateWorkItemActionAsync([FromBody] CreateWorkItemAction request)
    {
        return await PerformWorkItemActionAsync(request);
    }

    /// <summary>
    ///   Creates a new work item version
    /// </summary>
    /// <param name="request"></param>
    /// <returns>Ok with the results if successful, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpPost("action/create-version", Name = "CreateWorkItemVersionActionAsync")]
    [ProducesResponseType<WorkItemActionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<WorkItemActionResponse>> CreateWorkItemVersionActionAsync([FromBody] CreateWorkItemVersionAction request)
    {
        return await PerformWorkItemActionAsync(request);
    }

    /// <summary>
    ///  Deletes a work item
    /// </summary>
    /// <param name="request"></param>
    /// <returns>Ok with the results if successful, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpPost("action/delete", Name = "DeleteWorkItemActionAsync")]
    [ProducesResponseType<WorkItemActionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<WorkItemActionResponse>> DeleteWorkItemActionAsync([FromBody] DeleteWorkItemAction request)
    {
        return await PerformWorkItemActionAsync(request);
    }

    /// <summary>
    ///  Deletes a work item version
    /// </summary>
    /// <param name="request"></param>
    /// <returns>Ok with the results if successful, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpPost("action/delete-version", Name = "DeleteWorkItemVersionActionAsync")]
    [ProducesResponseType<WorkItemActionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<WorkItemActionResponse>> DeleteWorkItemVersionActionAsync([FromBody] DeleteWorkItemVersionAction request)
    {
        return await PerformWorkItemActionAsync(request);
    }

    /// <summary>
    ///  Moves a work item to a different queue
    /// </summary>
    /// <param name="request"></param>
    /// <returns>Ok with the results if successful, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpPost("action/move-queue", Name = "MoveWorkItemQueueActionAsync")]
    [ProducesResponseType<WorkItemActionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<WorkItemActionResponse>> MoveWorkItemQueueActionAsync([FromBody] MoveWorkItemQueueAction request)
    {
        return await PerformWorkItemActionAsync(request);
    }

    /// <summary>
    ///  Unassigns a work item
    /// </summary>
    /// <param name="request"></param>
    /// <returns>Ok with the results if successful, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpPost("action/unassign", Name = "UnassignWorkItemActionAsync")]
    [ProducesResponseType<WorkItemActionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<WorkItemActionResponse>> UnassignWorkItemActionAsync([FromBody] UnassignWorkItemAction request)
    {
        return await PerformWorkItemActionAsync(request);
    }
    #endregion

    // Ideally this would just be the controller, but NSwag's support for polymorphic types is basically non-existent as far as I can tell.
    private async ValueTask<ActionResult<WorkItemActionResponse>> PerformWorkItemActionAsync(WorkItemActionBase request)
    {
        BonesUser user = await GetCurrentBonesUserAsync();
        IRequest<CommandResponse> internalRequest = await request.ToInternalAsync(user, Sender);
        CommandResponse result = await Sender.Send(internalRequest);

        if (!result.Success)
        {
            return BadRequest(ErrorResponse.FromCommandResponse(result));
        }

        return await request.FromInternalAsync(result, user, Sender);
    }
}