using Bones.Api.Controllers.Base;
using Bones.Api.Models.Initiatives;
using Bones.Api.Models.Project;
using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.ProjectManagement;
using Bones.Database.DbSets.WorkItemManagement;
using Bones.Logic.Features.Initiatives;
using Bones.Logic.Features.WorkItems.Queue;

namespace Bones.Api.Controllers;

/// <summary>
///   Handles everything related to Managing Initiatives
/// </summary>
/// <param name="sender">Questy sender</param>
public sealed class InitiativeController(ISender sender) : AuthenticatedControllerBase(sender)
{
    #region GET
    /// <summary>
    ///     Gets a initiatives dashboard information
    /// </summary>
    /// <param name="initiativeId"></param>
    /// <returns>Ok with the results if successful, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpGet("{initiativeId:guid}/dashboard", Name = "GetInitiativeDashboardAsync")]
    [ProducesResponseType<GetInitiativeDashboardResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<Dictionary<string, string[]>>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<GetInitiativeDashboardResponse>> GetInitiativeDashboardAsync(Guid initiativeId)
    {
        BonesUser currentUser = await GetCurrentBonesUserAsync();
        QueryResponse<Initiative> initiativeResponse = await Sender.Send(new GetInitiativeById.Query(initiativeId, currentUser));

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
    ///     
    /// </summary>
    /// <param name="initiativeId">The ID of the initiative</param>
    /// <returns>The work item queues in the initiative.</returns>
    [HttpGet("{initiativeId:guid}/work-item-queues", Name = "GetWorkItemQueuesInInitiativeAsync")]
    [ProducesResponseType<List<GetWorkItemQueuesInInitiativeResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<List<GetWorkItemQueuesInInitiativeResponse>>> GetWorkItemQueuesInInitiativeAsync(Guid initiativeId)
    {
        QueryResponse<List<WorkItemQueue>> initiativeResponse = await Sender.Send(new GetWorkItemQueuesByInitiative.Query(initiativeId, await GetCurrentBonesUserAsync()));

        if (!initiativeResponse.Success || initiativeResponse.Result is null)
        {
            return BadRequest(ErrorResponse.FromQueryResponse(initiativeResponse));
        }

        return GetWorkItemQueuesInInitiativeResponse.FromInternalList(initiativeResponse.Result);
    }
    #endregion

    #region POST
    
    #endregion
}

