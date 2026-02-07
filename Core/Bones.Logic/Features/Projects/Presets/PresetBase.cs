using Bones.Database.DbSets.Accounts;
using Bones.Database.DbSets.Items;
using Bones.Database.DbSets.Projects;
using Bones.Logic.Features.Assets;
using Bones.Logic.Features.Items;
using Bones.Logic.Features.Initiatives;
using Bones.Logic.Features.Projects.Presets.Models;
using Bones.Logic.Features.Tasks.TaskQueues;
using Bones.Logic.Features.Tasks.Tasks;
using Bones.Shared.Backend.Enums;
using Bones.Database.DbSets.Items.Fields;
using Bones.Database.DbSets.Items.Layouts;
using Bones.Database.DbSets.Items.Types;
using Bones.Database.Operations.Items.Layouts;

namespace Bones.Logic.Features.Projects.Presets;

internal abstract class PresetBase
{
    internal abstract ProjectPreset Preset { get; }

    internal abstract string ProjectName { get; }

    internal abstract Dictionary<PresetFields, PresetFieldInfo> ItemFields { get; }

    internal abstract Dictionary<string, PresetLayoutInfo> ItemLayouts { get; }

    internal abstract Dictionary<string, PresetInitiativeInfo> ItemInitiatives { get; }

    internal abstract List<PresetTaskInfo> GetTasks();

    internal abstract List<PresetAssetInfo> GetAssets();

    internal async Task<Guid?> CreatePresetAsync(ISender sender, bool createTasksAndAssets, BonesUser requestingUser, CancellationToken cancellationToken)
    {
        CommandResponse projectCreation = await sender.Send(new CreateProject.Command(ProjectName, requestingUser), cancellationToken);
        if (!projectCreation.Success || projectCreation.Ids.Count == 0)
        {
            return null;
        }

        Guid projectId = projectCreation.Ids[nameof(Project)];

        if (!await CreatePresetFieldsAsync(sender, projectId, requestingUser, cancellationToken))
        {
            return null;
        }

        if (!await CreatePresetLayoutsAsync(sender, projectId, requestingUser, cancellationToken))
        {
            return null;
        }

        if (!await CreatePresetInitiativesAsync(sender, projectId, createTasksAndAssets, requestingUser, cancellationToken))
        {
            return null;
        }

        return projectId;
    }

    private async Task<bool> CreatePresetFieldsAsync(ISender sender, Guid projectId, BonesUser requestingUser, CancellationToken cancellationToken)
    {
        foreach ((PresetFields field, PresetFieldInfo fieldInfo) in ItemFields)
        {
            CommandResponse result = await sender.Send(new CreateItemField.Command(projectId, fieldInfo.Name, fieldInfo.IsRequired, fieldInfo.Type, fieldInfo.CanBeNegative, fieldInfo.PossibleValues, requestingUser), cancellationToken);
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

            CommandResponse result = await sender.Send(new CreateItemLayout.Command(projectId, layoutName, layoutInfo.LayoutUse, layoutInfo.FriendlyIdPrefix, fields, layoutInfo.AssigneeSlots, requestingUser), cancellationToken);
            if (!result.Success || result.Ids.Count == 0)
            {
                return false;
            }

            ItemLayout? layout = await sender.Send(new GetItemLayoutByIdDb.Query(result.Ids[nameof(ItemLayout)]), cancellationToken);

            if (layout?.Current is null)
            {
                return false;
            }

            ItemLayouts[layoutName] = layoutInfo with
            {
                LayoutId = layout.Id,
                LayoutVersionId = layout.Current.Id,
                AssigneeSlotIds = [.. layout.Current.ItemAssignmentSlots.Select(ad => ad.Id)],
            };
        }

        return true;
    }

    private async Task<bool> CreatePresetInitiativesAsync(ISender sender, Guid projectId, bool createTasksAndAssets, BonesUser requestingUser, CancellationToken cancellationToken)
    {
        foreach ((string initiativeName, PresetInitiativeInfo initiativeInfo) in ItemInitiatives)
        {
            CommandResponse result = await sender.Send(new CreateInitiative.Command(initiativeName, projectId, requestingUser), cancellationToken);
            if (!result.Success || result.Ids.Count == 0)
            {
                return false;
            }

            initiativeInfo.InitiativeId = result.Ids[nameof(Initiative)];

            foreach (KeyValuePair<string, PresetTaskQueueInfo> taskQueue in initiativeInfo.TaskQueues)
            {
                CommandResponse queueResult = await sender.Send(new CreateTaskQueue.Command(taskQueue.Key, initiativeInfo.InitiativeId.Value, requestingUser), cancellationToken);
                if (!queueResult.Success || queueResult.Ids.Count == 0)
                {
                    return false;
                }

                PresetTaskQueueInfo queue = taskQueue.Value;
                queue.TaskQueueId = queueResult.Ids[nameof(TaskQueue)];

                if (createTasksAndAssets)
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

                    foreach (PresetTaskInfo task in GetTasks())
                    {
                        CommandResponse taskCreation = await sender.Send(new CreateTaskInQueue.Command(
                            queue.TaskQueueId.Value,
                            task.Layout.LayoutId!.Value,
                            task.Layout.LayoutVersionId!.Value,
                            task.Title,
                            task.Fields.ToDictionary(kvp => kvp.Key.FieldVersionId!.Value, kvp => kvp.Value),
                            DateTimeOffset.UtcNow,
                            requestingUser
                        ), cancellationToken);

                        if (!taskCreation.Success || taskCreation.Ids.Count == 0)
                        {
                            return false;
                        }

                        task.TaskId = taskCreation.Ids[nameof(BonesTask)];
                        task.TaskVersionId = taskCreation.Ids[nameof(ItemVersion)];

                        if (task.ShouldAssignToCreator)
                        {
                            CommandResponse assignResponse = await sender.Send(new AssignTask.Command(task.TaskId.Value, task.Layout.AssigneeSlotIds!.First(), requestingUser.Id, task.AssignmentState, requestingUser), cancellationToken);
                            if (!assignResponse.Success)
                            {
                                return false;
                            }
                        }
                    }
                }
            }
        }

        return true;
    }
}