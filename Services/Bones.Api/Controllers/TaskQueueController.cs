using Bones.Api.Controllers.Base;
using Bones.Api.Models.TaskQueues;
using Bones.Database.DbSets.Accounts;
using Bones.Database.DbSets.Items.Types;
using Bones.Database.DbSets.Projects;
using Bones.Logic.Features.Items;
using Bones.Logic.Features.Tasks.TaskQueues;

namespace Bones.Api.Controllers;

/// <summary>
///   Handles task queue related operations
/// </summary>
/// <param name="sender"></param>
public class TaskQueueController(ISender sender) : AuthenticatedControllerBase(sender)
{
    #region GET
    /// <summary>
    ///   Gets the dashboard for a queue
    /// </summary>
    /// <param name="taskQueueId"></param>
    /// <returns>Ok with the results if successful, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpGet("{taskQueueId:guid}/dashboard", Name = "GetTaskQueueDashboardAsync")]
    [ProducesResponseType<GetTaskQueueDashboardResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async ValueTask<ActionResult<GetTaskQueueDashboardResponse>> GetTaskQueueDashboardAsync(Guid taskQueueId)
    {
        TaskQueue? queue = await Sender.Send(new GetTaskQueueById.Query(taskQueueId, await GetCurrentBonesUserAsync(), IncludeTasks: true));

        if (queue is null)
        {
            return NotFound(new ErrorResponse());
        }

        // The Item will not have been populated in the above query, need to pull that in
        foreach (BonesTask task in queue.BonesTasks)
        {
            task.Item = await Sender.Send(new GetItemById.Query(task.ItemId, await GetCurrentBonesUserAsync(), IncludeVersions: true));
        }

        return GetTaskQueueDashboardResponse.FromInternal(queue);
    }

    /// <summary>
    ///   Gets a  queue by its ID
    /// </summary>
    /// <param name="taskQueueId"></param>
    /// <returns>Ok with the results if successful, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpGet("{taskQueueId:guid}", Name = "GetTaskQueueByIdAsync")]
    [ProducesResponseType<GetTaskQueueByIdResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async ValueTask<ActionResult<GetTaskQueueByIdResponse>> GetTaskQueueByIdAsync(Guid taskQueueId)
    {
        TaskQueue? queue = await Sender.Send(new GetTaskQueueById.Query(taskQueueId, await GetCurrentBonesUserAsync(), IncludeInitiative: true));

        if (queue is null)
        {
            return BadRequest(new ErrorResponse());
        }

        return GetTaskQueueByIdResponse.FromInternal(queue);
    }
    #endregion

    #region POST
    /// <summary>
    ///   Creates a queue in an initiative
    /// </summary>
    /// <param name="request"></param>
    /// <returns>Ok with the results if successful, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpPost("create-in-initiative", Name = "CreateTaskQueueInInitiativeAsync")]
    [ProducesResponseType<Guid>(StatusCodes.Status200OK)]
    [ProducesResponseType<Dictionary<string, string[]>>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<Guid>> CreateTaskQueueInInitiativeAsync([FromBody] CreateTaskQueueInInitiativeRequest request)
    {
        BonesUser currentUser = await GetCurrentBonesUserAsync();
        CommandResponse resp = await Sender.Send(request.ToInternal(currentUser));

        if (!resp.Success || resp.Ids.Count == 0)
        {
            return BadRequest(resp.FailureReasons);
        }

        return resp.Ids[nameof(TaskQueue)];
    }
    #endregion


}