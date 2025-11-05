using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.AssetManagement;
using Bones.Database.DbSets.Items;
using Bones.Database.DbSets.ProjectManagement;
using Bones.Database.DbSets.WorkItemManagement;
using Bones.Logic.Features.Assets;
using Bones.Logic.Features.Item;
using Bones.Logic.Features.Initiatives;
using Bones.Logic.Features.Projects.Presets.Models;
using Bones.Logic.Features.WorkItems.Queue;
using Bones.Logic.Features.WorkItems.WorkItems;
using Bones.Shared.Backend.Enums;

namespace Bones.Logic.Features.Projects.Presets;

internal abstract class PresetBase
{
    internal abstract ProjectPreset Preset { get; }

    internal abstract string ProjectName { get; }

    internal abstract Dictionary<PresetFields, PresetFieldInfo> ItemFields { get; }

    internal abstract Dictionary<string, PresetLayoutInfo> ItemLayouts { get; }

    internal abstract Dictionary<string, PresetInitiativeInfo> ItemInitiatives { get; }

    internal abstract List<PresetWorkItemInfo> GetWorkItems();

    internal abstract List<PresetAssetInfo> GetAssets();

    internal async Task<bool> CreatePresetAsync(ISender sender, bool createWorkItemsAndAssets, BonesUser requestingUser, CancellationToken cancellationToken)
    {
        CommandResponse projectCreation = await sender.Send(new CreateProject.Command(ProjectName, requestingUser), cancellationToken);
        if (!projectCreation.Success || projectCreation.Ids.Count == 0)
        {
            return false;
        }

        Guid projectId = projectCreation.Ids[nameof(Project)];

        if (!await CreatePresetFieldsAsync(sender, projectId, requestingUser, cancellationToken))
        {
            return false;
        }

        if (!await CreatePresetLayoutsAsync(sender, projectId, requestingUser, cancellationToken))
        {
            return false;
        }

        if (!await CreatePresetInitiativesAsync(sender, projectId, createWorkItemsAndAssets, requestingUser, cancellationToken))
        {
            return false;
        }

        return true;
    }

    private async Task<bool> CreatePresetFieldsAsync(ISender sender, Guid projectId, BonesUser requestingUser, CancellationToken cancellationToken)
    {
        foreach ((PresetFields field, PresetFieldInfo fieldInfo) in ItemFields)
        {
            CommandResponse result = await sender.Send(new CreateItemField.Command(projectId, fieldInfo.Name, fieldInfo.IsRequired, fieldInfo.Type, fieldInfo.CanBeNegative, fieldInfo.PossibleValues, fieldInfo.GeoLocationType, fieldInfo.RequiredAddressFields, requestingUser), cancellationToken);
            if (!result.Success || result.Ids.Count == 0)
            {
                return false;
            }

            ItemFields[field] = fieldInfo with
            {
                FieldId = result.Ids[nameof(ItemField)],
                FieldVersionId = result.Ids[nameof(ItemFieldVersion)],
            };
        }

        return true;
    }

    private async Task<bool> CreatePresetLayoutsAsync(ISender sender, Guid projectId, BonesUser requestingUser, CancellationToken cancellationToken)
    {
        foreach ((string layoutName, PresetLayoutInfo layoutInfo) in ItemLayouts)
        {
            Dictionary<int, Guid> fields = [];

            foreach ((int fieldOrder, PresetFields presetField) in layoutInfo.Fields)
            {
                if (ItemFields.TryGetValue(presetField, out PresetFieldInfo? fieldInfo))
                {
                    // Another check to get the null reference warning out of here
                    if (fieldInfo.Created)
                    {
                        fields.Add(fieldOrder, fieldInfo.FieldVersionId.Value);
                    }
                    else
                    {
                        // This should never really happen unless the code is fucked up
                        throw new InvalidOperationException($"Field {presetField} is not created, but is used in layout {layoutName}");
                    }
                }
            }

            CommandResponse result = await sender.Send(new CreateItemLayout.Command(projectId, layoutName, layoutInfo.LayoutUse, layoutInfo.FriendlyIdPrefix, fields, requestingUser), cancellationToken);
            if (!result.Success || result.Ids.Count == 0)
            {
                return false;
            }

            ItemLayouts[layoutName] = layoutInfo with
            {
                LayoutId = result.Ids[nameof(ItemLayout)],
                LayoutVersionId = result.Ids[nameof(ItemLayoutVersion)],
            };
        }

        return true;
    }

    private async Task<bool> CreatePresetInitiativesAsync(ISender sender, Guid projectId, bool createWorkItemsAndAssets, BonesUser requestingUser, CancellationToken cancellationToken)
    {
        foreach ((string initiativeName, PresetInitiativeInfo initiativeInfo) in ItemInitiatives)
        {
            CommandResponse result = await sender.Send(new CreateInitiative.Command(initiativeName, projectId, requestingUser), cancellationToken);
            if (!result.Success || result.Ids.Count == 0)
            {
                return false;
            }

            initiativeInfo.InitiativeId = result.Ids[nameof(Initiative)];

            foreach (KeyValuePair<string, PresetWorkItemQueueInfo> workItemQueue in initiativeInfo.WorkItemQueues)
            {
                CommandResponse queueResult = await sender.Send(new CreateWorkItemQueue.Command(workItemQueue.Key, initiativeInfo.InitiativeId.Value, requestingUser), cancellationToken);
                if (!queueResult.Success || queueResult.Ids.Count == 0)
                {
                    return false;
                }

                PresetWorkItemQueueInfo queue = workItemQueue.Value;
                queue.WorkItemQueueId = queueResult.Ids[nameof(WorkItemQueue)];

                if (createWorkItemsAndAssets)
                {
                    foreach (PresetAssetInfo asset in GetAssets())
                    {
                        CommandResponse assetCreation = await sender.Send(new CreateAsset.Command(
                            asset.Layout.LayoutId!.Value,
                            asset.Layout.LayoutVersionId!.Value,
                            asset.Title,
                            asset.Fields.ToDictionary(kvp => kvp.Key.FieldVersionId!.Value, kvp => kvp.Value),
                            DateTimeOffset.UtcNow,
                            requestingUser
                        ), cancellationToken);

                        if (!assetCreation.Success || assetCreation.Ids.Count == 0)
                        {
                            return false;
                        }

                        asset.AssetId = assetCreation.Ids[nameof(Asset)];
                        asset.AssetVersionId = assetCreation.Ids[nameof(ItemVersion)];
                    }

                    foreach (PresetWorkItemInfo workItem in GetWorkItems())
                    {
                        CommandResponse workItemCreation = await sender.Send(new CreateWorkItemInQueue.Command(
                            queue.WorkItemQueueId.Value,
                            workItem.Layout.LayoutId!.Value,
                            workItem.Layout.LayoutVersionId!.Value,
                            workItem.Title,
                            workItem.Fields.ToDictionary(kvp => kvp.Key.FieldVersionId!.Value, kvp => kvp.Value),
                            DateTimeOffset.UtcNow,
                            requestingUser
                        ), cancellationToken);

                        if (!workItemCreation.Success || workItemCreation.Ids.Count == 0)
                        {
                            return false;
                        }

                        workItem.WorkItemId = workItemCreation.Ids[nameof(WorkItem)];
                        workItem.WorkItemVersionId = workItemCreation.Ids[nameof(ItemVersion)];
                    }
                }
            }
        }

        return true;
    }
}