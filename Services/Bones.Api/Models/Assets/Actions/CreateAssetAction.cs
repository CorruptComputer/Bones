using Bones.Api.Models.GenericItem;
using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.AssetManagement;
using Bones.Database.DbSets.GenericItems;
using Bones.Logic.Features.Assets;
using Bones.Logic.Features.GenericItem;
using Bones.Shared.Backend.Enums;
using Bones.Shared.Exceptions;

namespace Bones.Api.Models.Assets.Actions;

/// <summary>
///   Action to create a new asset.
/// </summary>
[JsonSerializable(typeof(CreateAssetAction))]
public sealed record class CreateAssetAction : AssetActionBase
{
    /// <summary>
    ///   The ID of the asset layout (not version, automatically uses the current version)
    /// </summary>
    public required Guid AssetLayoutId { get; init; }

    /// <summary>
    ///   The title of the asset
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    ///   The fields of the asset
    /// </summary>
    public required List<ItemValueModel> FieldValues { get; init; }

    internal override async Task<IRequest<CommandResponse>> ToInternalAsync(BonesUser user, ISender sender)
    {
        GenericItemLayout? layout = await sender.Send(new GetItemLayoutById.Query(AssetLayoutId, user));
        if (layout?.LatestVersion is null)
        {
            throw new BadRequestException("Layout not found")
            {
                RequestModel = nameof(CreateAssetAction),
                BadField = nameof(AssetLayoutId)
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
                    RequestModel = nameof(CreateAssetAction),
                    BadField = fieldVersion.Name
                };
            }

            fieldValues.Add(fieldVersion.Id, value);
        }

        return new CreateAsset.Command(layout.Id, layout.LatestVersion.Id, Title, fieldValues, ActionDateTime, user);
    }

    internal override Task<AssetActionResponse> FromInternalAsync(CommandResponse result, BonesUser user, ISender sender)
    {
        if (!result.Success)
        {
            throw new BonesException("CreateAssetAction.FromInternalAsync called with a failed CommandResponse");
        }

        if (result.Ids.Count == 0
            || !result.Ids.TryGetValue(nameof(Asset), out Guid assetId)
            || assetId == Guid.Empty
            || !result.Ids.TryGetValue(nameof(GenericItemVersion), out Guid assetVersionId)
            || assetVersionId == Guid.Empty)
        {
            throw new BonesException("No ID returned from command with successful status code: CreateAsset.Command");
        }

        return Task.FromResult(new AssetActionResponse
        {
            AssetId = assetId,
            AssetCurrentVersionId = assetVersionId,
        });
    }
}
