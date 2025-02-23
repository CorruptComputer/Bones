using Bones.Api.Models.WorkItems;
using Bones.Database.DbSets.WorkItemManagement;
using Bones.Logic.Features.WorkItems.Queue;

namespace Bones.Api.Controllers;

/// <summary>
///   Handles work items
/// </summary>
/// <param name="sender"></param>
public class WorkItemController(ISender sender) : BonesControllerBase(sender)
{
    #region GET
    /// <summary>
    ///     Gets the dashboard for a work item queue
    /// </summary>
    /// <param name="workItemQueueId"></param>
    /// <returns>Ok with the results if successful, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpGet("{workItemQueueId:guid}/dashboard", Name = "GetWorkItemQueueDashboardAsync")]
    [ProducesResponseType<GetWorkItemQueueDashboardResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<Dictionary<string, string[]>>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<GetWorkItemQueueDashboardResponse>> GetWorkItemQueueDashboardAsync(Guid workItemQueueId)
    {
        WorkItemQueue? queue = await Sender.Send(new GetWorkItemQueueById.Query(workItemQueueId, await GetCurrentBonesUserAsync()));

        if (queue is null)
        {
            return BadRequest("Queue not found");
        }

        return GetWorkItemQueueDashboardResponse.FromInternal(queue);
    }

    /// <summary>
    ///     Gets a work item queue by its ID
    /// </summary>
    /// <param name="workItemQueueId"></param>
    /// <returns>Ok with the results if successful, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpGet("{workItemQueueId:guid}", Name = "GetWorkItemQueueByIdAsync")]
    [ProducesResponseType<GetWorkItemQueueByIdResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<Dictionary<string, string[]>>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<GetWorkItemQueueByIdResponse>> GetWorkItemQueueByIdAsync(Guid workItemQueueId)
    {
        WorkItemQueue? queue = await Sender.Send(new GetWorkItemQueueById.Query(workItemQueueId, await GetCurrentBonesUserAsync()));

        if (queue is null)
        {
            return BadRequest("Queue not found");
        }

        return GetWorkItemQueueByIdResponse.FromInternal(queue);
    }
    #endregion
}