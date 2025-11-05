using System.Diagnostics.CodeAnalysis;
using Bones.Shared.Backend.Enums;

namespace Bones.Logic.Features.Projects.Presets.Models;

internal record PresetLayoutInfo
{
    internal required ItemLayoutUse LayoutUse { get; init; }

    internal required string FriendlyIdPrefix { get; init; }

    internal required Dictionary<int, PresetFields> Fields { get; init; }

    internal required Dictionary<int, (string name, AssignmentType assType, SelectionType selType)> AssigneeDefinitions { get; init; }

    [MemberNotNullWhen(true, nameof(LayoutId))]
    [MemberNotNullWhen(true, nameof(LayoutVersionId))]
    internal bool Created => LayoutId.HasValue && LayoutVersionId.HasValue;

    /// <summary>
    ///   After the layout is created, this will be set to the ID of the layout
    /// </summary>
    internal Guid? LayoutId { get; set; }

    /// <summary>
    ///   After the layout is created, this will be set to the ID of the layout
    /// </summary>
    internal Guid? LayoutVersionId { get; set; }
}
