using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.GenericItems;
using Bones.Logic.Features.GenericItem;
using Bones.Shared.Backend.Enums;
using Bones.Shared.Enums;

namespace Bones.Logic.Features.Projects.ProjectPresets;

/// <inheritdoc />
public class DevelopmentPreset : IProjectPreset
{
    /// <inheritdoc />
    public ProjectPreset Preset => ProjectPreset.Development;

    /// <inheritdoc />
    public async Task<bool> CreatePresetLayoutsAsync(ISender sender, Guid projectId, BonesUser requestingUser, CancellationToken cancellationToken)
    {
        // Add all the needed fields to the project
        CommandResponse titleFieldCreation = await sender.Send(new CreateItemField.Command(projectId, "Title", true, FieldType.TextField, null, null, null, null, requestingUser), cancellationToken);
        if (!titleFieldCreation.Success || titleFieldCreation.Id == null)
        {
            return false;
        }

        CommandResponse featureAreaFieldCreation = await sender.Send(new CreateItemField.Command(projectId, "Feature Area", true, FieldType.ValueList, null, new()
            {
                { "Account", StringValueMatchingType.CaseInvariant },
                { "Cart", StringValueMatchingType.CaseInvariant },
                { "Item", StringValueMatchingType.CaseInvariant },
                { "Other", StringValueMatchingType.CaseInvariant }
            }, null, null, requestingUser), cancellationToken);
        if (!featureAreaFieldCreation.Success || featureAreaFieldCreation.Id == null)
        {
            return false;
        }

        CommandResponse isCrashFieldCreation = await sender.Send(new CreateItemField.Command(projectId, "Is Crash?", true, FieldType.Boolean, null, null, null, null, requestingUser), cancellationToken);
        if (!isCrashFieldCreation.Success || isCrashFieldCreation.Id == null)
        {
            return false;
        }

        CommandResponse reproductionStepsFieldCreation = await sender.Send(new CreateItemField.Command(projectId, "Reproduction Steps", true, FieldType.TextBox, null, null, null, null, requestingUser), cancellationToken);
        if (!reproductionStepsFieldCreation.Success || reproductionStepsFieldCreation.Id == null)
        {
            return false;
        }

        CommandResponse descriptionFieldCreation = await sender.Send(new CreateItemField.Command(projectId, "Description", true, FieldType.TextBox, null, null, null, null, requestingUser), cancellationToken);
        if (!descriptionFieldCreation.Success || descriptionFieldCreation.Id == null)
        {
            return false;
        }


        // Get the fields by their IDs so we can get the current version IDs
        GenericItemField? titleField = await sender.Send(new GetItemFieldById.Query(titleFieldCreation.Id.Value, requestingUser), cancellationToken);
        if (titleField?.CurrentVersion?.Id == null)
        {
            return false;
        }

        GenericItemField? featureAreaField = await sender.Send(new GetItemFieldById.Query(featureAreaFieldCreation.Id.Value, requestingUser), cancellationToken);
        if (featureAreaField?.CurrentVersion?.Id == null)
        {
            return false;
        }

        GenericItemField? isCrashField = await sender.Send(new GetItemFieldById.Query(isCrashFieldCreation.Id.Value, requestingUser), cancellationToken);
        if (isCrashField?.CurrentVersion?.Id == null)
        {
            return false;
        }

        GenericItemField? reproductionStepsField = await sender.Send(new GetItemFieldById.Query(reproductionStepsFieldCreation.Id.Value, requestingUser), cancellationToken);
        if (reproductionStepsField?.CurrentVersion?.Id == null)
        {
            return false;
        }

        GenericItemField? descriptionField = await sender.Send(new GetItemFieldById.Query(descriptionFieldCreation.Id.Value, requestingUser), cancellationToken);
        if (descriptionField?.CurrentVersion?.Id == null)
        {
            return false;
        }

        // Add the layouts
        CommandResponse bugLayout = await sender.Send(new CreateItemLayout.Command(projectId, "Bug", ItemLayoutUses.WorkItems, "BUG", new()
            {
                { 0, titleField.CurrentVersion.Id },
                { 1, featureAreaField.CurrentVersion.Id },
                { 2, isCrashField.CurrentVersion.Id },
                { 3, reproductionStepsField.CurrentVersion.Id },
                { 4, descriptionField.CurrentVersion.Id }
            }, requestingUser), cancellationToken);

        if (!bugLayout.Success)
        {
            return false;
        }

        CommandResponse featureLayout = await sender.Send(new CreateItemLayout.Command(projectId, "Feature", ItemLayoutUses.WorkItems, "FEAT", new()
            {
                { 0, titleField.CurrentVersion.Id },
                { 1, featureAreaField.CurrentVersion.Id },
                { 2, descriptionField.CurrentVersion.Id }

            }, requestingUser), cancellationToken);

        if (!featureLayout.Success)
        {
            return false;
        }

        return true;
    }

    private sealed record FieldVersionIds(Guid FieldId, Guid VersionId)
    {
        public static implicit operator FieldVersionIds((Guid FieldId, Guid VersionId) tuple) => new(tuple.FieldId, tuple.VersionId);
    }
}
