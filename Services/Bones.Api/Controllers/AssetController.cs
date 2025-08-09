using Bones.Api.Controllers.Base;
using Bones.Api.Models.Assets;
using Bones.Api.Models.Assets.Actions;
using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.AssetManagement;
using Bones.Database.DbSets.GenericItems;
using Bones.Logic.Features.Assets;
using Bones.Logic.Features.GenericItem;
using Bones.Shared.Enums;

namespace Bones.Api.Controllers;

/// <summary>
///   Handles everything related to Managing Assets
/// </summary>
/// <param name="sender">Questy sender</param>
public sealed class AssetController(ISender sender) : AuthenticatedControllerBase(sender)
{
    #region GET
    /// <summary>
    ///   Gets the dashboard for an asset layout, which includes the layout and all assets that use it.
    /// </summary>
    /// <param name="GenericItemLayoutId"></param>
    /// <returns>Ok with the results if successful, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpGet("Layout/{GenericItemLayoutId:guid}/dashboard", Name = "GetAssetLayoutDashboardAsync")]
    [ProducesResponseType<GetAssetLayoutDashboardResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async ValueTask<ActionResult<GetAssetLayoutDashboardResponse>> GetAssetLayoutDashboardAsync(Guid GenericItemLayoutId)
    {
        GenericItemLayout? layout = await Sender.Send(new GetItemLayoutById.Query(GenericItemLayoutId, await GetCurrentBonesUserAsync()));
        if (layout is null || layout.LatestVersion?.LayoutUse.HasFlag(ItemLayoutUse.Assets) != true)
        {
            return NotFound(new ErrorResponse());
        }

        List<Asset>? assets = await Sender.Send(new GetAssetsByLayoutId.Query(GenericItemLayoutId, await GetCurrentBonesUserAsync()));

        assets ??= [];

        return GetAssetLayoutDashboardResponse.FromInternal(layout, assets);
    }


    /// <summary>
    ///   Gets an asset by its ID
    /// </summary>
    /// <param name="AssetId"></param>
    /// <returns>Ok with the results if successful, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpGet("{AssetId:guid}", Name = "GetAssetByIdAsync")]
    [ProducesResponseType<GetAssetByIdResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async ValueTask<ActionResult<GetAssetByIdResponse>> GetAssetByIdAsync(Guid AssetId)
    {
        Asset? item = await Sender.Send(new GetAssetById.Query(AssetId, await GetCurrentBonesUserAsync()));

        await Task.CompletedTask;

        if (item is null)
        {
            return NotFound(new ErrorResponse());
        }

        return GetAssetByIdResponse.FromInternal(item);
    }
    #endregion

    #region POST
    /// <summary>
    ///   Creates a new asset
    /// </summary>
    /// <param name="request"></param>
    /// <returns>Ok with the results if successful, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpPost("action/create", Name = "CreateAssetActionAsync")]
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
    [HttpPost("action/create-version", Name = "CreateAssetVersionActionAsync")]
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
    [HttpPost("action/delete", Name = "DeleteAssetActionAsync")]
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
    [HttpPost("action/delete-version", Name = "DeleteAssetVersionActionAsync")]
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
