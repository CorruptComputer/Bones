using Bones.Api.Models.Project;
using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.ProjectManagement;
using Bones.Logic.Features.Projects.Initiatives;

namespace Bones.Api.Controllers;

/// <summary>
///   Handles everything related to Managing Initiatives
/// </summary>
/// <param name="sender">MediatR sender</param>
public sealed class InitiativeController(ISender sender) : BonesControllerBase(sender)
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
    #endregion

    #region POST

    #endregion
}

