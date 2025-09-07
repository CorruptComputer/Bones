using Bones.Api.Models.GenericItem;
using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.GenericItems;
using Bones.Logic.Features.GenericItem;
using Bones.Logic.Features.WorkItems.WorkItems;
using Bones.Shared.Backend.Enums;
using Bones.Shared.Exceptions;

namespace Bones.Api.Models.WorkItems.Actions;

/// <summary>
///   Action to create a new version of a work item.
/// </summary>
[JsonSerializable(typeof(CreateWorkItemVersionAction))]
public sealed record class CreateWorkItemVersionAction : WorkItemActionBase
{
    /// <summary>
    ///   The ID of the work item to perform the action on
    /// </summary>
    public required Guid WorkItemId { get; init; }

    /// <summary>
    ///   The ID of the work item layout (not version, automatically uses the current version)
    /// </summary>
    public required Guid WorkItemLayoutId { get; init; }

    /// <summary>
    ///   The title of the work item
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    ///   The fields of the work item
    /// </summary>
    public required List<ItemValueModel> FieldValues { get; init; }

    internal override async Task<IRequest<CommandResponse>> ToInternalAsync(BonesUser user, ISender sender)
    {
        GenericItemLayout? layout = await sender.Send(new GetItemLayoutById.Query(WorkItemLayoutId, user));
        if (layout?.LatestVersion is null)
        {
            throw new BadRequestException("Layout not found")
            {
                RequestModel = nameof(CreateWorkItemAction),
                BadField = nameof(WorkItemLayoutId)
            };
        }

        Dictionary<Guid, object?> fieldValues = [];

        foreach (GenericItemFieldVersion fieldVersion in layout.LatestVersion.FieldLinks.Select(x => x.FieldVersion))
        {
            ItemValueModel? fieldValue = FieldValues.FirstOrDefault(x => x.FieldVersionId == fieldVersion.Id);

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
                throw new BadRequestException($"Field {fieldVersion.Name} is required")
                {
                    RequestModel = nameof(CreateWorkItemAction),
                    BadField = fieldVersion.Name
                };
            }

            fieldValues.Add(fieldVersion.Id, value);
        }

        return new CreateWorkItemVersion.Command(WorkItemId, layout.Id, Title, fieldValues, ActionDateTime, user);
    }

    internal override Task<WorkItemActionResponse> FromInternalAsync(CommandResponse result, BonesUser user, ISender sender)
    {
        if (!result.Success)
        {
            throw new BonesException("CreateWorkItemAction.FromInternalAsync called with a failed CommandResponse");
        }

        if (result.Ids.Count == 0
            || !result.Ids.TryGetValue(nameof(GenericItemVersion), out Guid workItemVersionId)
            || workItemVersionId == Guid.Empty)
        {
            throw new BonesException("No ID returned from command with successful status code: CreateWorkItemInQueue.Command");
        }

        return Task.FromResult(new WorkItemActionResponse
        {
            WorkItemId = WorkItemId,
            WorkItemCurrentVersionId = workItemVersionId,
        });
    }
}
