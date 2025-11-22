using Bones.Api.Models.Item;
using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.Items;
using Bones.Logic.Features.Item;
using Bones.Logic.Features.Tasks.Tasks;
using Bones.Shared.Backend.Enums;
using Bones.Shared.Exceptions;

namespace Bones.Api.Models.Tasks.Actions;

/// <summary>
///   Action to create a new version of a .
/// </summary>
[JsonSerializable(typeof(CreateTaskVersionAction))]
public sealed record class CreateTaskVersionAction : TaskActionBase
{
    /// <summary>
    ///   The ID of the  to perform the action on
    /// </summary>
    public required Guid TaskId { get; init; }

    /// <summary>
    ///   The ID of the  layout (not version, automatically uses the current version)
    /// </summary>
    public required Guid TaskLayoutId { get; init; }

    /// <summary>
    ///   The title of the
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    ///   The fields of the
    /// </summary>
    public required List<ItemValueModel> FieldValues { get; init; }

    internal override async Task<IRequest<CommandResponse>> ToInternalAsync(BonesUser user, ISender sender)
    {
        ItemLayout? layout = await sender.Send(new GetItemLayoutById.Query(TaskLayoutId, user));
        if (layout?.Current is null)
        {
            throw new BadRequestException("Layout not found")
            {
                RequestModel = nameof(CreateTaskAction),
                BadField = nameof(TaskLayoutId)
            };
        }

        Dictionary<Guid, object?> fieldValues = [];

        foreach (ItemFieldVersion fieldVersion in layout.Current.FieldLinks.Select(x => x.FieldVersion))
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
                    RequestModel = nameof(CreateTaskAction),
                    BadField = fieldVersion.Name
                };
            }

            fieldValues.Add(fieldVersion.Id, value);
        }

        return new CreateTaskVersion.Command(TaskId, layout.Id, Title, fieldValues, ActionDateTime, user);
    }

    internal override Task<TaskActionResponse> FromInternalAsync(CommandResponse result, BonesUser user, ISender sender)
    {
        if (!result.Success)
        {
            throw new BonesException("CreateTaskAction.FromInternalAsync called with a failed CommandResponse");
        }

        if (result.Ids.Count == 0
            || !result.Ids.TryGetValue(nameof(ItemVersion), out Guid taskVersionId)
            || taskVersionId == Guid.Empty)
        {
            throw new BonesException("No ID returned from command with successful status code: CreateTaskInQueue.Command");
        }

        return Task.FromResult(new TaskActionResponse
        {
            TaskId = TaskId,
            TaskCurrentVersionId = taskVersionId,
        });
    }
}
