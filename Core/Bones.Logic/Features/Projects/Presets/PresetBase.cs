using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.GenericItems;
using Bones.Database.DbSets.ProjectManagement;
using Bones.Logic.Features.GenericItem;
using Bones.Shared.Enums;

namespace Bones.Logic.Features.Projects.Presets;

internal abstract class PresetBase
{
    internal abstract ProjectPreset Preset { get; }

    internal abstract string PresetName { get; }

    internal abstract Dictionary<PresetFields, PresetFieldInfo> ItemFields { get; }

    internal abstract Dictionary<string, PresetLayoutInfo> ItemLayouts { get; }

    internal async Task<bool> CreatePresetAsync(ISender sender, BonesUser requestingUser, CancellationToken cancellationToken)
    {
        CommandResponse projectCreation = await sender.Send(new CreateProject.Command(PresetName, requestingUser), cancellationToken);
        if (!projectCreation.Success || projectCreation.Ids.Count == 0)
        {
            return false;
        }

        if (!await CreatePresetFieldsAsync(sender, projectCreation.Ids[nameof(Project)], requestingUser, cancellationToken))
        {
            return false;
        }

        if (!await CreatePresetLayoutsAsync(sender, projectCreation.Ids[nameof(Project)], requestingUser, cancellationToken))
        {
            return false;
        }

        return true;
    }

    private async Task<bool> CreatePresetFieldsAsync(ISender sender, Guid projectId, BonesUser requestingUser, CancellationToken cancellationToken)
    {
        foreach ((PresetFields _, PresetFieldInfo fieldInfo) in ItemFields)
        {
            CommandResponse result = await sender.Send(new CreateItemField.Command(projectId, fieldInfo.Name, fieldInfo.IsRequired, fieldInfo.Type, fieldInfo.CanBeNegative, fieldInfo.PossibleValues, fieldInfo.GeoLocationType, fieldInfo.RequiredAddressFields, requestingUser), cancellationToken);
            if (!result.Success || result.Ids.Count == 0)
            {
                return false;
            }

            fieldInfo.FieldId = result.Ids[nameof(GenericItemField)];
        }

        return true;
    }

    private async Task<bool> CreatePresetLayoutsAsync(ISender sender, Guid projectId, BonesUser requestingUser, CancellationToken cancellationToken)
    {
        foreach ((string layoutName, PresetLayoutInfo layoutInfo) in ItemLayouts)
        {
            Dictionary<uint, Guid> fields = [];

            foreach ((uint fieldOrder, PresetFields presetField) in layoutInfo.Fields)
            {
                if (ItemFields.TryGetValue(presetField, out PresetFieldInfo? fieldInfo))
                {
                    // Another check to get the null reference warning out of here
                    if (fieldInfo.Created)
                    {
                        GenericItemField? field = await sender.Send(new GetItemFieldById.Query(fieldInfo.FieldId.Value, requestingUser), cancellationToken);
                        if (field?.LatestVersion is null)
                        {
                            return false;
                        }

                        fields.Add(fieldOrder, field.LatestVersion.Id);
                    }
                    else
                    {
                        // This should never really happen unless the code is fucked up
                        throw new InvalidOperationException($"Field {presetField} is not created, but is used in layout {layoutName}");
                    }
                }
            }

            CommandResponse result = await sender.Send(new CreateItemLayout.Command(projectId, layoutName, layoutInfo.EnabledFor, layoutInfo.FriendlyIdPrefix, fields, requestingUser), cancellationToken);
            if (!result.Success || result.Ids.Count == 0)
            {
                return false;
            }
        }

        return true;
    }


}