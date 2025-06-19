using Bones.Api.Controllers.Base;
using Bones.Api.Models.WorkItems;
using Bones.Database.DbSets.GenericItems;
using Bones.Database.DbSets.WorkItemManagement;
using Bones.Logic.Features.GenericItem;
using Bones.Logic.Features.WorkItems.WorkItems;
using Bones.Shared.Backend.Enums;
using Bones.Shared.Exceptions;

namespace Bones.Api.Controllers;

/// <summary>
///   Handles work items
/// </summary>
/// <param name="sender"></param>
public class WorkItemController(ISender sender) : AuthenticatedControllerBase(sender)
{
    #region GET
    /// <summary>
    ///   Gets a work item by its ID
    /// </summary>
    /// <param name="workItemId"></param>
    /// <returns>Ok with the results if successful, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpGet("{workItemId:guid}", Name = "GetWorkItemByIdAsync")]
    [ProducesResponseType<GetWorkItemByIdResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async ValueTask<ActionResult<GetWorkItemByIdResponse>> GetWorkItemByIdAsync(Guid workItemId)
    {
        WorkItem? item = await Sender.Send(new GetWorkItemById.Query(workItemId, await GetCurrentBonesUserAsync()));

        if (item is null)
        {
            return NotFound(new ErrorResponse());
        }

        return GetWorkItemByIdResponse.FromInternal(item);
    }
    #endregion

    #region POST
    /// <summary>
    ///   Creates a work item in the specified queue
    /// </summary>
    /// <param name="workItemQueueId"></param>
    /// <param name="request"></param>
    /// <returns>Ok with the results if successful, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpPost("create-in-queue", Name = "CreateWorkItemInQueueAsync")]
    [ProducesResponseType<Guid>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<Guid>> CreateWorkItemInQueueAsync([FromQuery] Guid workItemQueueId, [FromBody] CreateWorkItemRequest request)
    {
        GenericItemLayout? layout = await Sender.Send(new GetItemLayoutById.Query(request.WorkItemLayoutId, await GetCurrentBonesUserAsync()));
        if (layout?.CurrentVersion is null)
        {
            return BadRequest(new ErrorResponse("Layout not found"));
        }

        Dictionary<Guid, object?> fieldValues = [];

        foreach (GenericItemLayoutFieldVersionLink fieldVersionLink in layout.CurrentVersion.FieldLinks)
        {
            GenericItemFieldVersion fieldVersion = fieldVersionLink.FieldVersion;
            ItemValueModel? fieldValue = request.FieldValues.FirstOrDefault(x => x.FieldVersionId == fieldVersion.Id);

            object? value = fieldVersion.Type switch
            {
                FieldType.TextField or FieldType.TextBox or FieldType.ValueList => fieldValue?.StrValue,
                FieldType.Integer => fieldValue?.IntValue,
                FieldType.Decimal => fieldValue?.DecimalValue,
                FieldType.Boolean => fieldValue?.BoolValue,
                FieldType.DateTime => fieldValue?.DateTimeValue,
                //FieldType.GeoLocation => fieldValue?.StrValue, // TODO: Handle this
                _ => null
            };

            if (value is null && fieldVersion.IsRequired)
            {
                return BadRequest(new ErrorResponse($"Field {fieldVersion.Name} is required"));
            }

            fieldValues.Add(fieldVersion.Id, value);
        }

        CommandResponse result = await Sender.Send(new CreateWorkItemInQueue.Command(workItemQueueId, layout.Id, layout.CurrentVersion.Id, request.Title, fieldValues, await GetCurrentBonesUserAsync()));
        if (!result.Success)
        {
            return BadRequest(ErrorResponse.FromCommandResponse(result));
        }

        if (!result.Id.HasValue)
        {
            throw new BonesException("No ID returned from command: CreateWorkItemInQueue.Command");
        }

        return result.Id.Value;
    }

    /// <summary>
    ///   Creates a new version of a work item
    /// </summary>
    /// <param name="workItemQueueId"></param>
    /// <param name="request"></param>
    /// <returns>Ok with the results if successful, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpPost("{workItemQueueId:guid}/create-work-item", Name = "CreateWorkItemVersionAsync")]
    [ProducesResponseType<Guid>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<Guid>> CreateWorkItemVersionAsync(Guid workItemQueueId, [FromBody] CreateWorkItemRequest request)
    {
        GenericItemLayout? layout = await Sender.Send(new GetItemLayoutById.Query(request.WorkItemLayoutId, await GetCurrentBonesUserAsync()));
        if (layout?.CurrentVersion is null)
        {
            return BadRequest(new ErrorResponse("Layout not found"));
        }

        Dictionary<Guid, object?> fieldValues = [];

        foreach (GenericItemLayoutFieldVersionLink fieldVersionLink in layout.CurrentVersion.FieldLinks)
        {
            GenericItemFieldVersion fieldVersion = fieldVersionLink.FieldVersion;
            ItemValueModel? fieldValue = request.FieldValues.FirstOrDefault(x => x.FieldVersionId == fieldVersion.Id);

            object? value = fieldVersion.Type switch
            {
                FieldType.TextField or FieldType.TextBox or FieldType.ValueList => fieldValue?.StrValue,
                FieldType.Integer => fieldValue?.IntValue,
                FieldType.Decimal => fieldValue?.DecimalValue,
                FieldType.Boolean => fieldValue?.BoolValue,
                FieldType.DateTime => fieldValue?.DateTimeValue,
                //FieldType.GeoLocation => fieldValue?.StrValue, // TODO: Handle this
                _ => null
            };

            if (value is null && fieldVersion.IsRequired)
            {
                return BadRequest(new ErrorResponse($"Field {fieldVersion.Name} is required"));
            }

            fieldValues.Add(fieldVersion.Id, value);
        }

        CommandResponse result = await Sender.Send(new CreateWorkItemInQueue.Command(workItemQueueId, layout.Id, layout.CurrentVersion.Id, request.Title, fieldValues, await GetCurrentBonesUserAsync()));
        if (!result.Success)
        {
            return BadRequest(ErrorResponse.FromCommandResponse(result));
        }

        if (!result.Id.HasValue)
        {
            throw new BonesException("No ID returned from command: CreateWorkItemInQueue.Command");
        }

        return result.Id.Value;
    }
    #endregion
}