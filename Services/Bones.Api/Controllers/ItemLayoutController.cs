using Bones.Api.Controllers.Base;
using Bones.Api.Models.Item;
using Bones.Api.Models.Project;
using Bones.Database.DbSets.Items;
using Bones.Logic.Features.Item;

namespace Bones.Api.Controllers;

/// <summary>
///   Handles everything related to managing item layouts and their fields
/// </summary>
/// <param name="sender">Questy sender</param>
public sealed class ItemLayoutController(ISender sender) : AuthenticatedControllerBase(sender)
{
    #region GET
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
        QueryResponse<ItemField?> fieldResponse = await Sender.Send(new GetItemFieldById.Query(fieldId, await GetCurrentBonesUserAsync()));

        if (!fieldResponse.Success || fieldResponse.Result is null)
        {
            return BadRequest(ErrorResponse.FromQueryResponse(fieldResponse));
        }

        return GetLatestItemFieldVersionResponse.FromInternal(fieldResponse.Result);
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
        QueryResponse<ItemLayout?> layoutResponse = await Sender.Send(new GetItemLayoutById.Query(layoutId, await GetCurrentBonesUserAsync()));

        if (!layoutResponse.Success || layoutResponse.Result is null)
        {
            return BadRequest(ErrorResponse.FromQueryResponse(layoutResponse));
        }

        ItemLayoutVersion? layoutVersion = null;
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
            layoutVersion = layoutResponse.Result.Current;
            if (layoutVersion is null)
            {
                return BadRequest(new ErrorResponse("No versions found for this layout"));
            }
        }

        return GetItemLayoutVersionResponse.FromInternal(layoutVersion, layoutResponse.Result.Project.Id, layoutResponse.Result.FriendlyIdPrefix, layoutResponse.Result.Current?.Version ?? 0);
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
        QueryResponse<ItemLayout?> layoutResponse = await Sender.Send(new GetItemLayoutById.Query(layoutId, await GetCurrentBonesUserAsync()));

        if (!layoutResponse.Success || layoutResponse.Result is null)
        {
            return BadRequest(ErrorResponse.FromQueryResponse(layoutResponse));
        }

        return GetLatestItemLayoutVersionFieldsResponse.FromInternal(layoutResponse.Result);
    }
    #endregion

    #region POST
    /// <summary>
    ///   Creates a new item field in a project
    /// </summary>
    /// <param name="request">The request</param>
    /// <returns>Created if created, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpPost("fields/create", Name = "CreateItemFieldAsync")]
    [ProducesResponseType<Guid>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<Guid>> CreateItemFieldAsync([FromBody] CreateItemFieldRequest request)
    {
        CommandResponse response = await Sender.Send(request.ToInternal(await GetCurrentBonesUserAsync()));
        if (!response.Success)
        {
            return BadRequest(ErrorResponse.FromCommandResponse(response));
        }

        return response.Ids[nameof(ItemField)];
    }

    /// <summary>
    ///   Creates a new item field version in a project
    /// </summary>
    /// <param name="fieldId">The ID of the field to add this version to</param>
    /// <param name="request">The request</param>
    /// <returns>Created if created, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpPost("fields/{fieldId:guid}", Name = "CreateItemFieldVersionAsync")]
    [ProducesResponseType<Guid>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<Guid>> CreateItemFieldVersionAsync(Guid fieldId, [FromBody] CreateItemFieldVersionRequest request)
    {
        CommandResponse response = await Sender.Send(request.ToInternal(fieldId, await GetCurrentBonesUserAsync()));
        if (!response.Success)
        {
            return BadRequest(ErrorResponse.FromCommandResponse(response));
        }

        return response.Ids[nameof(ItemFieldVersion)];
    }

    /// <summary>
    ///   Creates a new item layout in a project
    /// </summary>
    /// <param name="request">The request</param>
    /// <returns>Created if created, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpPost("layouts/create", Name = "CreateItemLayoutAsync")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<Guid>> CreateItemLayoutAsync([FromBody] CreateItemLayoutRequest request)
    {
        CommandResponse response = await Sender.Send(request.ToInternal(await GetCurrentBonesUserAsync()));
        if (!response.Success)
        {
            return BadRequest(ErrorResponse.FromCommandResponse(response));
        }

        return Ok();
    }

    /// <summary>
    ///   Creates a new item layout version in a project
    /// </summary>
    /// <param name="layoutId">The ID of the layout to add this version to</param>
    /// <param name="request">The request</param>
    /// <returns>Created if created, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpPost("layouts/{layoutId:guid}", Name = "CreateItemLayoutVersionAsync")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult> CreateItemLayoutVersionAsync(Guid layoutId, [FromBody] CreateItemLayoutVersionRequest request)
    {
        CommandResponse response = await Sender.Send(request.ToInternal(layoutId, await GetCurrentBonesUserAsync()));
        if (!response.Success)
        {
            return BadRequest(ErrorResponse.FromCommandResponse(response));
        }

        return Ok();
    }
    #endregion
}
