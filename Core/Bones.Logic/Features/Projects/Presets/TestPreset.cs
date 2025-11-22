using Bones.Logic.Features.Projects.Presets.Models;
using Bones.Shared.Backend.Enums;

namespace Bones.Logic.Features.Projects.Presets;

internal class TestPreset : PresetBase
{
    internal override ProjectPreset Preset => ProjectPreset.Test;

    internal override string ProjectName => "Test Project";

    internal override Dictionary<PresetFields, PresetFieldInfo> ItemFields { get; } = new()
    {
        {
            PresetFields.RequiredSmallText,
            new PresetFieldInfo
            {
                Name = "Required Small Text",
                IsRequired = true,
                Type = FieldType.TextField
            }
        },
        {
            PresetFields.RequiredInteger,
            new PresetFieldInfo
            {
                Name = "Required Integer",
                IsRequired = true,
                Type = FieldType.Integer,
                CanBeNegative = true
            }
        },
        {
            PresetFields.OptionalPositiveDecimal,
            new PresetFieldInfo
            {
                Name = "Optional Positive Decimal",
                IsRequired = false,
                Type = FieldType.Decimal,
                CanBeNegative = false
            }
        },
        {
            PresetFields.RequiredValueList,
            new PresetFieldInfo
            {
                Name = "Required Value List",
                IsRequired = true,
                Type = FieldType.ValueList,
                PossibleValues = new()
                {
                    { "Option 1", StringValueMatchingType.Exact },
                    { "Option 2", StringValueMatchingType.CaseInvariant },
                    { "Option 3", StringValueMatchingType.Soundex }
                }
            }
        },
        {
            PresetFields.OptionalLargeText,
            new PresetFieldInfo
            {
                Name = "Optional Large Text",
                IsRequired = false,
                Type = FieldType.TextBox
            }
        },
        {
            PresetFields.OptionalBoolean,
            new PresetFieldInfo
            {
                Name = "Optional Boolean",
                IsRequired = false,
                Type = FieldType.Boolean
            }
        },
        {
            PresetFields.OptionalDateTime,
            new PresetFieldInfo
            {
                Name = "Optional Date Time",
                IsRequired = false,
                Type = FieldType.DateTime
            }
        }
    };

    internal override Dictionary<string, PresetLayoutInfo> ItemLayouts { get; } = new()
    {
        {
            "Test Task",
            new PresetLayoutInfo
            {
                LayoutUse = ItemLayoutUse.Tasks,
                FriendlyIdPrefix = "TT",
                Fields = new()
                {
                    { 0, PresetFields.RequiredSmallText },
                    { 1, PresetFields.RequiredInteger },
                    { 2, PresetFields.OptionalPositiveDecimal },
                    { 3, PresetFields.RequiredValueList },
                    { 4, PresetFields.OptionalLargeText },
                    { 5, PresetFields.OptionalBoolean },
                    { 6, PresetFields.OptionalDateTime }
                },
                AssigneeSlots = new()
                {
                    { 0, ("Slot one", AssignmentType.User, SelectionType.Single, ["To do", "In Progress", "Done"]) },
                    { 1, ("Slot two", AssignmentType.User, SelectionType.Single, ["To do", "In Progress", "Done"]) },
                }
            }
        },
        {
            "Test Asset",
            new PresetLayoutInfo
            {
                LayoutUse = ItemLayoutUse.Assets,
                FriendlyIdPrefix = "TA",
                Fields = new()
                {
                    { 0, PresetFields.RequiredSmallText },
                    { 1, PresetFields.RequiredInteger },
                    { 4, PresetFields.OptionalLargeText }
                },
                AssigneeSlots = new()
                {
                    { 0, ("Owning Group", AssignmentType.Role, SelectionType.Single, ["To do", "In Progress", "Done"]) },
                }
            }
        }
    };

    internal override Dictionary<string, PresetInitiativeInfo> ItemInitiatives { get; } = new()
    {
        {
            "Test Initiative",
            new PresetInitiativeInfo
            {
                TaskQueues = new()
                {
                    {
                        "Test Queue",
                        new()
                    }
                }
            }
        }
    };

    internal override List<PresetAssetInfo> GetAssets()
    {
        List<PresetAssetInfo> assets = [];
        for (int i = 0; i < 10; i++)
        {
            assets.Add(new PresetAssetInfo
            {
                Title = $"Test Asset {i + 1}",
                Layout = ItemLayouts["Test Asset"],
                Fields = new Dictionary<PresetFieldInfo, object?>
                {
                    { ItemFields[PresetFields.RequiredSmallText], "Test Value" },
                    { ItemFields[PresetFields.RequiredInteger], -123L },
                    { ItemFields[PresetFields.OptionalLargeText], "Large Text\n\n\n\n\n\n\n\n\nLarge Text" }
                }
            });
        }

        return assets;
    }

    internal override List<PresetTaskInfo> GetTasks()
    {
        List<PresetTaskInfo> tasks = [];
        for (int i = 0; i < 100; i++)
        {
            tasks.Add(new PresetTaskInfo
            {
                Title = $"Test Task {i + 1}",
                Layout = ItemLayouts["Test Task"],
                Fields = new Dictionary<PresetFieldInfo, object?>
                {
                    { ItemFields[PresetFields.RequiredSmallText], "Test Value" },
                    { ItemFields[PresetFields.RequiredInteger], -123L },
                    { ItemFields[PresetFields.OptionalPositiveDecimal], 3.14d },
                    { ItemFields[PresetFields.RequiredValueList], "Option 1" },
                    { ItemFields[PresetFields.OptionalLargeText], "Large Text\n\n\n\n\n\n\n\n\nLarge Text" },
                    { ItemFields[PresetFields.OptionalBoolean], true },
                    { ItemFields[PresetFields.OptionalDateTime], DateTimeOffset.UtcNow }
                },
                ShouldAssignToCreator = i == 0,
                AssignmentState = "To do"
            });
        }

        return tasks;
    }
}
