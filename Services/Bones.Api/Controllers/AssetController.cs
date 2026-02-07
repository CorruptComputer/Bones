using Bones.Api.Controllers.Base;
using Bones.Api.Models.Assets;
using Bones.Api.Models.Assets.Actions;
using Bones.Api.Models.Assignment;
using Bones.Database.DbSets.Accounts;
using Bones.Database.DbSets.Items.Assignments;
using Bones.Database.DbSets.Items.Types;
using Bones.Logic.Features.Assets;
using Bones.Logic.Features.Items;

namespace Bones.Api.Controllers;

/// <summary>
///   Handles everything related to Managing Assets
/// </summary>
/// <param name="sender">Questy sender</param>
public sealed class AssetController(ISender sender) : AuthenticatedControllerBase(sender)
{
    #region GET
    /// <summary>
    ///   Gets an asset by its ID
    /// </summary>
    /// <param name="assetId"></param>
    /// <returns>Ok with the results if successful, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpGet("{assetId:guid}", Name = "GetAssetByIdAsync")]
    [ProducesResponseType<GetAssetByIdResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async ValueTask<ActionResult<GetAssetByIdResponse>> GetAssetByIdAsync(Guid assetId)
    {
        Asset? asset = await Sender.Send(new GetAssetById.Query(assetId, await GetCurrentBonesUserAsync(), IncludeItem: true));

        await Task.CompletedTask;

        if (asset is null)
        {
            return NotFound(new ErrorResponse());
        }

        // The Item will not have been populated in the above query, need to pull that in
        asset.Item = await Sender.Send(new GetItemById.Query(asset.ItemId, await GetCurrentBonesUserAsync(), IncludeVersions: true));

        return GetAssetByIdResponse.FromInternal(asset);
    }

    /// <summary>
    ///   Gets the assignment information for the latest version of an asset
    /// </summary>
    /// <param name="assetId">The ID of the asset</param>
    /// <returns>The currently assigned users and the layout needed to display them.</returns>
    [HttpGet("{assetId:guid}/assignments", Name = "GetLatestAssetAssignmentsAsync")]
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
    #endregion

    #region POST
    /// <summary>
    ///   Creates a new asset
    /// </summary>
    /// <param name="request"></param>
    /// <returns>Ok with the results if successful, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpPost("create", Name = "CreateAssetActionAsync")]
    [ProducesResponseType<AssetActionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<AssetActionResponse>> CreateAssetActionAsync([FromBody] CreateAssetAction request)
    {
        return await PerformAssetActionAsync(request);
    }

    /// <summary>
    ///   Creates a new asset version
    /// </summary>
    /// <param name="request"></param>
    /// <returns>Ok with the results if successful, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpPost("{AssetId:guid}/create-version", Name = "CreateAssetVersionActionAsync")]
    [ProducesResponseType<AssetActionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<AssetActionResponse>> CreateAssetVersionActionAsync([FromBody] CreateAssetVersionAction request)
    {
        return await PerformAssetActionAsync(request);
    }

    /// <summary>
    ///  Deletes an asset
    /// </summary>
    /// <param name="request"></param>
    /// <returns>Ok with the results if successful, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpPost("{AssetId:guid}/delete", Name = "DeleteAssetActionAsync")]
    [ProducesResponseType<AssetActionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<AssetActionResponse>> DeleteAssetActionAsync([FromBody] DeleteAssetAction request)
    {
        return await PerformAssetActionAsync(request);
    }

    /// <summary>
    ///  Deletes an asset version
    /// </summary>
    /// <param name="request"></param>
    /// <returns>Ok with the results if successful, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpPost("{AssetId:guid}/delete-version", Name = "DeleteAssetVersionActionAsync")]
    [ProducesResponseType<AssetActionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<AssetActionResponse>> DeleteAssetVersionActionAsync([FromBody] DeleteAssetVersionAction request)
    {
        return await PerformAssetActionAsync(request);
    }
    #endregion

    // Ideally this would just be the controller, but NSwag's support for polymorphic types is basically non-existent as far as I can tell.
    private async ValueTask<ActionResult<AssetActionResponse>> PerformAssetActionAsync(AssetActionBase request)
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
