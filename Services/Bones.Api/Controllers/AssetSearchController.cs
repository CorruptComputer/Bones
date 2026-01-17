using Bones.Api.Controllers.Base;
using Bones.Api.Models.Assets;
using Bones.Api.Models.Assets.Actions;
using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.AssetManagement;
using Bones.Database.DbSets.Items;
using Bones.Logic.Features.Assets;
using Bones.Logic.Features.Items;
using Bones.Shared.Backend.Enums;

namespace Bones.Api.Controllers;

/// <summary>
///   Handles everything related to Managing Assets
/// </summary>
/// <param name="sender">Questy sender</param>
public sealed class AssetSearchController(ISender sender) : AuthenticatedControllerBase(sender)
{
    #region GET
    /// <summary>
    ///   Gets the dashboard for an asset layout, which includes the layout and all assets that use it.
    /// </summary>
    /// <param name="ItemLayoutId"></param>
    /// <returns>Ok with the results if successful, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpGet("by-layout/{ItemLayoutId:guid}", Name = "GetAssetsByLayoutId")]
    [ProducesResponseType<GetAssetLayoutDashboardResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async ValueTask<ActionResult<GetAssetLayoutDashboardResponse>> GetAssetLayoutDashboardAsync(Guid ItemLayoutId)
    {
        ItemLayout? layout = await Sender.Send(new GetItemLayoutById.Query(ItemLayoutId, await GetCurrentBonesUserAsync()));
        if (layout is null || layout.Current?.LayoutUse.HasFlag(ItemLayoutUse.Assets) != true)
        {
            return NotFound(new ErrorResponse());
        }

        List<Asset>? assets = await Sender.Send(new GetAssetsByLayoutId.Query(ItemLayoutId, await GetCurrentBonesUserAsync()));

        assets ??= [];

        return GetAssetLayoutDashboardResponse.FromInternal(layout, assets);
    }
    #endregion
}
