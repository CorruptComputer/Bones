using Bones.Shared.Backend.Enums;
using Bones.Shared.Enums;

namespace Bones.Logic.Features.Projects.Presets;

internal class DevelopmentPreset : PresetBase
{
    internal override ProjectPreset Preset => ProjectPreset.Development;

    internal override string PresetName => "Development";

    internal override Dictionary<PresetFields, PresetFieldInfo> ItemFields => new()
    {
        {
            PresetFields.Title,
            new PresetFieldInfo
            {
                Name = "Title",
                IsRequired = true,
                Type = FieldType.TextField
            }
        },
        {
            PresetFields.FeatureArea,
            new PresetFieldInfo
            {
                Name = "Feature Area",
                IsRequired = true,
                Type = FieldType.ValueList,
                PossibleValues = new()
                {
                    { "Account", StringValueMatchingType.CaseInvariant },
                    { "Cart", StringValueMatchingType.CaseInvariant },
                    { "Item", StringValueMatchingType.CaseInvariant },
                    { "Other", StringValueMatchingType.CaseInvariant }
                }
            }
        },
        {
            PresetFields.IsCrash,
            new PresetFieldInfo
            {
                Name = "Is Crash?",
                IsRequired = true,
                Type = FieldType.Boolean
            }
        },
        {
            PresetFields.ReproductionSteps,
            new PresetFieldInfo
            {
                Name = "Reproduction Steps",
                IsRequired = true,
                Type = FieldType.TextBox
            }
        },
        {
            PresetFields.Description,
            new PresetFieldInfo
            {
                Name = "Description",
                IsRequired = true,
                Type = FieldType.TextBox
            }
        }
    };


    internal override Dictionary<string, PresetLayoutInfo> ItemLayouts => new()
    {
        {
            "Bug",
            new PresetLayoutInfo
            {
                EnabledFor = ItemLayoutUses.WorkItems,
                FriendlyIdPrefix = "BUG",
                Fields = new()
                {
                    { 0, PresetFields.Title },
                    { 1, PresetFields.FeatureArea },
                    { 2, PresetFields.IsCrash },
                    { 3, PresetFields.ReproductionSteps },
                    { 4, PresetFields.Description }
                }
            }
        },
        {
            "Feature",
            new PresetLayoutInfo
            {
                EnabledFor = ItemLayoutUses.WorkItems,
                FriendlyIdPrefix = "FEAT",
                Fields = new()
                {
                    { 0, PresetFields.Title },
                    { 1, PresetFields.FeatureArea },
                    { 2, PresetFields.Description }
                }
            }
        }
    };
}
