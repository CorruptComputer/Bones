using Bones.Api.Controllers.Base;
using Bones.Api.Models.Initiatives;
using Bones.Api.Models.Project;
using Bones.Database.DbSets.Accounts;
using Bones.Database.DbSets.Projects;
using Bones.Logic.Features.Initiatives;
using Bones.Logic.Features.Tasks.TaskQueues;

namespace Bones.Api.Controllers;

/// <summary>
///   Handles everything related to Managing Initiatives
/// </summary>
/// <param name="sender">Questy sender</param>
public sealed class InitiativeController(ISender sender) : AuthenticatedControllerBase(sender)
{
    #region GET
    /// <summary>
    ///   Gets a initiatives dashboard information
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
            ProjectId = initiative.ProjectId,
            TaskQueueCount = initiative.Queues.Count,
            TaskQueues = initiative.Queues.Select(i =>
                new GetInitiativeDashboardResponse.TaskQueueListModel
                {
                    TaskQueueId = i.Id,
                    TaskQueueName = i.Name,
                    TaskCount = i.BonesTasks.Count
                })
        };

        return resp;
    }

    /// <summary>
    ///   Gets the task queues in an initiative
    /// </summary>
    /// <param name="initiativeId">The ID of the initiative</param>
    /// <returns>The task queues in the initiative.</returns>
    [HttpGet("{initiativeId:guid}/task-queues", Name = "GetTaskQueuesInInitiativeAsync")]
    [ProducesResponseType<List<GetTaskQueuesInInitiativeResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<List<GetTaskQueuesInInitiativeResponse>>> GetTaskQueuesInInitiativeAsync(Guid initiativeId)
    {
        QueryResponse<List<TaskQueue>> initiativeResponse = await Sender.Send(new GetTaskQueuesByInitiative.Query(initiativeId, await GetCurrentBonesUserAsync()));

        if (!initiativeResponse.Success || initiativeResponse.Result is null)
        {
            return BadRequest(ErrorResponse.FromQueryResponse(initiativeResponse));
        }

        return GetTaskQueuesInInitiativeResponse.FromInternalList(initiativeResponse.Result);
    }
    #endregion

    #region POST

    #endregion
}

