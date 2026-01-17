using Bones.Api.Controllers.Base;
using Bones.Api.Models.ItemVersions;
using Bones.Database.DbSets.Items;
using Bones.Logic.Features.Items;
using Microsoft.EntityFrameworkCore.Query;

namespace Bones.Api.Controllers;

/// <summary>
///   Handles everything related to Managing Item Versions
/// </summary>
/// <param name="sender">Questy sender</param>
public sealed class ItemVersionController(ISender sender) : AuthenticatedControllerBase(sender)
{
    #region GET
    /// <summary>
    ///   Gets an item version by its ID
    /// </summary>
    /// <param name="ItemVersionId"></param>
    /// <returns>Ok with the results if successful, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpGet("{ItemVersionId:guid}", Name = "GetItemVersionByIdAsync")]
    [ProducesResponseType<GetItemVersionByIdResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async ValueTask<ActionResult<GetItemVersionByIdResponse>> GetItemVersionByIdAsync(Guid ItemVersionId)
    {
        ItemVersion? itemVersion = await Sender.Send(new GetItemVersionById.Query(ItemVersionId, await GetCurrentBonesUserAsync(), IncludeLayoutVersion: true, IncludeValues: true, IncludeAssignees: true));

        await Task.CompletedTask;

        if (itemVersion is null)
        {
            return NotFound(new ErrorResponse());
        }

        return GetItemVersionByIdResponse.FromInternal(itemVersion);
    }
    #endregion
}
