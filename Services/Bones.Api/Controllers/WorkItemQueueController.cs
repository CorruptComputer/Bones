using Bones.Api.Controllers.Base;
using Bones.Api.Models.Initiatives;
using Bones.Api.Models.WorkItems;
using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.GenericItems;
using Bones.Database.DbSets.WorkItemManagement;
using Bones.Logic.Features.GenericItem;
using Bones.Logic.Features.WorkItems.Queue;
using Bones.Logic.Features.WorkItems.WorkItems;
using Bones.Shared.Backend.Enums;
using Bones.Shared.Exceptions;

namespace Bones.Api.Controllers;

/// <summary>
///   Handles work items
/// </summary>
/// <param name="sender"></param>
public class WorkItemQueueController(ISender sender) : AuthenticatedControllerBase(sender)
{
    #region GET
    /// <summary>
    ///   Gets the dashboard for a work item queue
    /// </summary>
    /// <param name="workItemQueueId"></param>
    /// <returns>Ok with the results if successful, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpGet("{workItemQueueId:guid}/dashboard", Name = "GetWorkItemQueueDashboardAsync")]
    [ProducesResponseType<GetWorkItemQueueDashboardResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async ValueTask<ActionResult<GetWorkItemQueueDashboardResponse>> GetWorkItemQueueDashboardAsync(Guid workItemQueueId)
    {
        WorkItemQueue? queue = await Sender.Send(new GetWorkItemQueueById.Query(workItemQueueId, await GetCurrentBonesUserAsync()));

        if (queue is null)
        {
            return NotFound(new ErrorResponse());
        }

        return GetWorkItemQueueDashboardResponse.FromInternal(queue);
    }

    /// <summary>
    ///   Gets a work item queue by its ID
    /// </summary>
    /// <param name="workItemQueueId"></param>
    /// <returns>Ok with the results if successful, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpGet("{workItemQueueId:guid}", Name = "GetWorkItemQueueByIdAsync")]
    [ProducesResponseType<GetWorkItemQueueByIdResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async ValueTask<ActionResult<GetWorkItemQueueByIdResponse>> GetWorkItemQueueByIdAsync(Guid workItemQueueId)
    {
        WorkItemQueue? queue = await Sender.Send(new GetWorkItemQueueById.Query(workItemQueueId, await GetCurrentBonesUserAsync()));

        if (queue is null)
        {
            return BadRequest(new ErrorResponse());
        }

        return GetWorkItemQueueByIdResponse.FromInternal(queue);
    }
    #endregion

    #region POST
    /// <summary>
    ///   Creates a queue in an initiative
    /// </summary>
    /// <param name="request"></param>
    /// <returns>Ok with the results if successful, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpPost("create-in-initiative", Name = "CreateQueueInInitiativeAsync")]
    [ProducesResponseType<Guid>(StatusCodes.Status200OK)]
    [ProducesResponseType<Dictionary<string, string[]>>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<Guid>> CreateQueueInInitiativeAsync([FromBody] CreateQueueInInitiativeRequest request)
    {
        BonesUser currentUser = await GetCurrentBonesUserAsync();
        CommandResponse resp = await Sender.Send(request.ToInternal(currentUser));

        if (!resp.Success || resp.Id == null)
        {
            return BadRequest(resp.FailureReasons);
        }

        return resp.Id;
    }
    #endregion


}