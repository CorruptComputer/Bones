using Bones.Api.Controllers.Base;
using Bones.Api.Models.Assignment;
using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.AssetManagement;
using Bones.Database.DbSets.Items;
using Bones.Database.DbSets.TaskManagement;
using Bones.Logic.Features.Assets;
using Bones.Logic.Features.Tasks.Tasks;

namespace Bones.Api.Controllers;

/// <summary>
///   Handles everything related to Managing Assets
/// </summary>
/// <param name="sender">Questy sender</param>
public sealed class AssignmentController(ISender sender) : AuthenticatedControllerBase(sender)
{
    /// <summary>
    ///   Gets the assignment information for the latest version of an asset
    /// </summary>
    /// <param name="assetId">The ID of the asset</param>
    /// <returns>The currently assigned users and the layout needed to display them.</returns>
    [HttpGet("asset/{assetId:guid}/latest", Name = "GetLatestAssetAssignmentsAsync")]
    [ProducesResponseType<GetLatestAssignmentsResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<GetLatestAssignmentsResponse>> GetLatestAssetAssignmentsAsync(Guid assetId)
    {
        BonesUser currentUser = await GetCurrentBonesUserAsync();
        List<ItemAssignmentSlot>? assigneeSlots = await Sender.Send(new GetAssetAssigneeSlotsById.Query(assetId, currentUser));
        List<ItemAssignee>? assignees = await Sender.Send(new GetAssetCurrentAssigneesById.Query(assetId, currentUser));

        // It'll be an empty list if the item exists but has no assignee slots
        if (assigneeSlots is null)
        {
            return BadRequest(new ErrorResponse("Asset not found."));
        }

        return GetLatestAssignmentsResponse.FromInternal(assigneeSlots, assignees ?? []);
    }

    /// <summary>
    ///   Gets the assignment information for the latest version of a task
    /// </summary>
    /// <param name="taskId">The ID of the task</param>
    /// <returns>The currently assigned users and the layout needed to display them.</returns>
    [HttpGet("task/{taskId:guid}/latest", Name = "GetLatestTaskAssignmentsAsync")]
    [ProducesResponseType<GetLatestAssignmentsResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<GetLatestAssignmentsResponse>> GetLatestTaskAssignmentsAsync(Guid taskId)
    {
        BonesUser currentUser = await GetCurrentBonesUserAsync();
        List<ItemAssignmentSlot>? assigneeSlots = await Sender.Send(new GetTaskAssigneeSlotsById.Query(taskId, currentUser));
        List<ItemAssignee>? assignees = await Sender.Send(new GetTaskCurrentAssigneesById.Query(taskId, currentUser));

        // It'll be an empty list if the item exists but has no assignee slots
        if (assigneeSlots is null)
        {
            return BadRequest(new ErrorResponse("Task not found."));
        }

        return GetLatestAssignmentsResponse.FromInternal(assigneeSlots, assignees ?? []);
    }
}
