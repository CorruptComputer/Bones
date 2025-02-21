using Bones.Api.Models.Project;
using Bones.Database.DbSets.GenericItems;
using Bones.Logic.Features.GenericItem;

namespace Bones.Api.Controllers;

/// <summary>
///   Handles everything related to Managing Generic Items
/// </summary>
/// <param name="sender">MediatR sender</param>
public sealed class GenericItemController(ISender sender) : BonesControllerBase(sender)
{
        /// <summary>
    ///     Gets the latest version of a field
    /// </summary>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="fieldId">The ID of the field</param>
    /// <returns>The latest version of the requested field.</returns>
    [HttpGet("{projectId:guid}/fields/{fieldId:guid}/latest", Name = "GetLatestItemFieldVersionAsync")]
    [ProducesResponseType<GetLatestItemFieldVersionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<GetLatestItemFieldVersionResponse>> GetLatestItemFieldVersionAsync(Guid projectId, Guid fieldId)
    {
        QueryResponse<GenericItemField?> fieldResponse = await Sender.Send(new GetItemFieldById.Query(fieldId, await GetCurrentBonesUserAsync()));

        if (!fieldResponse.Success || fieldResponse.Result is null)
        {
            return BadRequest(ErrorResponse.FromQueryResponse(fieldResponse));
        }

        return GetLatestItemFieldVersionResponse.FromInternal(fieldResponse.Result);
    }

    /// <summary>
    ///     Gets the latest version of a layout
    /// </summary>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="layoutId">The ID of the layout</param>
    /// <returns>The latest version of the requested layout.</returns>
    [HttpGet("{projectId:guid}/layouts/{layoutId:guid}/latest", Name = "GetLatestItemLayoutVersionAsync")]
    [ProducesResponseType<GetLatestItemLayoutVersionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<GetLatestItemLayoutVersionResponse>> GetLatestItemLayoutVersionAsync(Guid projectId, Guid layoutId)
    {
        QueryResponse<GenericItemLayout?> layoutResponse = await Sender.Send(new GetItemLayoutById.Query(layoutId, await GetCurrentBonesUserAsync()));

        if (!layoutResponse.Success || layoutResponse.Result is null)
        {
            return BadRequest(ErrorResponse.FromQueryResponse(layoutResponse));
        }

        return GetLatestItemLayoutVersionResponse.FromInternal(layoutResponse.Result);
    }
}
