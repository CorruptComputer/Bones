using Bones.Shared.Backend.Enums;

namespace Bones.Logic.Features.Projects.Presets;

internal class PresetLayoutInfo
{
    internal required ItemLayoutUses EnabledFor { get; init; }

    internal required string FriendlyIdPrefix { get; init; }

    internal required Dictionary<uint, PresetFields> Fields { get; init; }
}
