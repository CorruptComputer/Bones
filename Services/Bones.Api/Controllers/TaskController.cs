using Bones.Api.Controllers.Base;
using Bones.Api.Models.Tasks;
using Bones.Api.Models.Tasks.Actions;
using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.TaskManagement;
using Bones.Logic.Features.Tasks.Tasks;

namespace Bones.Api.Controllers;

/// <summary>
///   Handles task related operations
/// </summary>
/// <param name="sender"></param>
public class TaskController(ISender sender) : AuthenticatedControllerBase(sender)
{
    #region GET
    /// <summary>
    ///   Gets a  by its ID
    /// </summary>
    /// <param name="taskId"></param>
    /// <returns>Ok with the results if successful, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpGet("{taskId:guid}", Name = "GetTaskByIdAsync")]
    [ProducesResponseType<GetTaskByIdResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async ValueTask<ActionResult<GetTaskByIdResponse>> GetTaskByIdAsync(Guid taskId)
    {
        BonesTask? item = await Sender.Send(new GetTaskById.Query(taskId, await GetCurrentBonesUserAsync()));

        if (item is null)
        {
            return NotFound(new ErrorResponse());
        }

        return GetTaskByIdResponse.FromInternal(item);
    }
    #endregion

    #region POST
    /// <summary>
    ///   Assigns a
    /// </summary>
    /// <param name="request"></param>
    /// <returns>Ok with the results if successful, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpPost("action/assign", Name = "AssignTaskActionAsync")]
    [ProducesResponseType<TaskActionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<TaskActionResponse>> AssignTaskActionAsync([FromBody] AssignTaskAction request)
    {
        return await PerformTaskActionAsync(request);
    }

    /// <summary>
    ///   Creates a new
    /// </summary>
    /// <param name="request"></param>
    /// <returns>Ok with the results if successful, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpPost("action/create", Name = "CreateTaskActionAsync")]
    [ProducesResponseType<TaskActionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<TaskActionResponse>> CreateTaskActionAsync([FromBody] CreateTaskAction request)
    {
        return await PerformTaskActionAsync(request);
    }

    /// <summary>
    ///   Creates a new  version
    /// </summary>
    /// <param name="request"></param>
    /// <returns>Ok with the results if successful, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpPost("action/create-version", Name = "CreateTaskVersionActionAsync")]
    [ProducesResponseType<TaskActionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<TaskActionResponse>> CreateTaskVersionActionAsync([FromBody] CreateTaskVersionAction request)
    {
        return await PerformTaskActionAsync(request);
    }

    /// <summary>
    ///  Deletes a
    /// </summary>
    /// <param name="request"></param>
    /// <returns>Ok with the results if successful, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpPost("action/delete", Name = "DeleteTaskActionAsync")]
    [ProducesResponseType<TaskActionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<TaskActionResponse>> DeleteTaskActionAsync([FromBody] DeleteTaskAction request)
    {
        return await PerformTaskActionAsync(request);
    }

    /// <summary>
    ///  Deletes a  version
    /// </summary>
    /// <param name="request"></param>
    /// <returns>Ok with the results if successful, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpPost("action/delete-version", Name = "DeleteTaskVersionActionAsync")]
    [ProducesResponseType<TaskActionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<TaskActionResponse>> DeleteTaskVersionActionAsync([FromBody] DeleteTaskVersionAction request)
    {
        return await PerformTaskActionAsync(request);
    }

    /// <summary>
    ///  Moves a  to a different queue
    /// </summary>
    /// <param name="request"></param>
    /// <returns>Ok with the results if successful, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpPost("action/move-queue", Name = "MoveTaskQueueActionAsync")]
    [ProducesResponseType<TaskActionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<TaskActionResponse>> MoveTaskQueueActionAsync([FromBody] MoveTaskToQueueAction request)
    {
        return await PerformTaskActionAsync(request);
    }

    /// <summary>
    ///  Unassigns a
    /// </summary>
    /// <param name="request"></param>
    /// <returns>Ok with the results if successful, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpPost("action/unassign", Name = "UnassignTaskActionAsync")]
    [ProducesResponseType<TaskActionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<TaskActionResponse>> UnassignTaskActionAsync([FromBody] UnassignTaskAction request)
    {
        return await PerformTaskActionAsync(request);
    }
    #endregion

    // Ideally this would just be the controller, but NSwag's support for polymorphic types is basically non-existent as far as I can tell.
    private async ValueTask<ActionResult<TaskActionResponse>> PerformTaskActionAsync(TaskActionBase request)
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