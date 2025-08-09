using Bones.Api.Controllers.Base;
using Bones.Api.Models.GenericItem;
using Bones.Database.DbSets.GenericItems;
using Bones.Logic.Features.GenericItem;
using Bones.Shared.Enums;

namespace Bones.Api.Controllers;

/// <summary>
///   Handles everything related to Managing Generic Items
/// </summary>
/// <param name="sender">Questy sender</param>
public sealed class GenericItemController(ISender sender) : AuthenticatedControllerBase(sender)
{
    /// <summary>
    ///   Gets the latest version of a field
    /// </summary>
    /// <param name="fieldId">The ID of the field</param>
    /// <returns>The latest version of the requested field.</returns>
    [HttpGet("fields/{fieldId:guid}/latest", Name = "GetLatestItemFieldVersionAsync")]
    [ProducesResponseType<GetLatestItemFieldVersionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<GetLatestItemFieldVersionResponse>> GetLatestItemFieldVersionAsync(Guid fieldId)
    {
        QueryResponse<GenericItemField?> fieldResponse = await Sender.Send(new GetItemFieldById.Query(fieldId, await GetCurrentBonesUserAsync()));

        if (!fieldResponse.Success || fieldResponse.Result is null)
        {
            return BadRequest(ErrorResponse.FromQueryResponse(fieldResponse));
        }

        return GetLatestItemFieldVersionResponse.FromInternal(fieldResponse.Result);
    }

    /// <summary>
    ///   Gets the item layouts for a project, optionally filtered by the uses they are enabled for
    /// </summary>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="layoutUse"></param>
    /// <returns>The latest version of the requested layout.</returns>
    [HttpGet("{projectId:guid}/layouts", Name = "GetProjectLayoutsAsync")]
    [ProducesResponseType<List<GetProjectLayoutsResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<List<GetProjectLayoutsResponse>>> GetProjectLayoutsAsync(Guid projectId, [FromQuery] ItemLayoutUse? layoutUse = null)
    {
        QueryResponse<List<GenericItemLayout>> layouts = await Sender.Send(new GetItemLayoutsByProject.Query(projectId, await GetCurrentBonesUserAsync()));

        if (!layouts.Success || layouts.Result is null)
        {
            return BadRequest(ErrorResponse.FromQueryResponse(layouts));
        }

        return GetProjectLayoutsResponse.FromInternalList(layouts.Result, layoutUse);
    }

    /// <summary>
    ///   Get an ItemLayoutVersion by its LayoutID and version, or the latest version if no version is specified
    /// </summary>
    /// <param name="layoutId">The ID of the layout</param>
    /// <param name="requestedVersion">The version if you want a specific version instead of the latest</param>
    /// <returns>The latest version of the requested layout.</returns>
    [HttpGet("layouts/{layoutId:guid}/latest", Name = "GetLatestItemLayoutVersionAsync")]
    [HttpGet("layouts/{layoutId:guid}/{requestedVersion:long}", Name = "GetItemLayoutVersionAsync")]
    [ProducesResponseType<GetItemLayoutVersionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<GetItemLayoutVersionResponse>> GetItemLayoutVersionAsync(Guid layoutId, long? requestedVersion = null)
    {
        QueryResponse<GenericItemLayout?> layoutResponse = await Sender.Send(new GetItemLayoutById.Query(layoutId, await GetCurrentBonesUserAsync()));

        if (!layoutResponse.Success || layoutResponse.Result is null)
        {
            return BadRequest(ErrorResponse.FromQueryResponse(layoutResponse));
        }

        GenericItemLayoutVersion? layoutVersion = null;
        if (requestedVersion.HasValue && requestedVersion.Value > 0)
        {
            layoutVersion = layoutResponse.Result.Versions.FirstOrDefault(v => v.Version == requestedVersion.Value);
            if (layoutVersion is null)
            {
                return BadRequest(new ErrorResponse("Requested version not found"));
            }
        }
        else
        {
            layoutVersion = layoutResponse.Result.LatestVersion;
            if (layoutVersion is null)
            {
                return BadRequest(new ErrorResponse("No versions found for this layout"));
            }
        }

        return GetItemLayoutVersionResponse.FromInternal(layoutVersion, layoutResponse.Result.Project.Id, layoutResponse.Result.FriendlyIdPrefix, layoutResponse.Result.LatestVersion?.Version ?? 0);
    }

    /// <summary>
    ///   Gets the latest version of a layout
    /// </summary>
    /// <param name="layoutId">The ID of the layout</param>
    /// <returns>The latest version of the requested layout.</returns>
    [HttpGet("layouts/{layoutId:guid}/latest/fields", Name = "GetLatestItemLayoutVersionFieldsAsync")]
    [ProducesResponseType<List<GetLatestItemLayoutVersionFieldsResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<List<GetLatestItemLayoutVersionFieldsResponse>>> GetLatestItemLayoutVersionFieldsAsync(Guid layoutId)
    {
        QueryResponse<GenericItemLayout?> layoutResponse = await Sender.Send(new GetItemLayoutById.Query(layoutId, await GetCurrentBonesUserAsync()));

        if (!layoutResponse.Success || layoutResponse.Result is null)
        {
            return BadRequest(ErrorResponse.FromQueryResponse(layoutResponse));
        }

        return GetLatestItemLayoutVersionFieldsResponse.FromInternal(layoutResponse.Result);
    }
}
