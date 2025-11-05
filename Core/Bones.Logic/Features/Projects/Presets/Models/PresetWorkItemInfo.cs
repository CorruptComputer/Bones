using System.Diagnostics.CodeAnalysis;

namespace Bones.Logic.Features.Projects.Presets.Models;

internal record PresetWorkItemInfo
{
    internal required string Title { get; init; }

    internal required PresetLayoutInfo Layout { get; init; }

    internal required Dictionary<PresetFieldInfo, object?> Fields { get; init; }

    [MemberNotNullWhen(true, nameof(WorkItemId))]
    [MemberNotNullWhen(true, nameof(WorkItemVersionId))]
    internal bool Created => WorkItemId.HasValue && WorkItemVersionId.HasValue;

    /// <summary>
    ///   After the work item is created, this will be set to the ID of the work item
    /// </summary>
    internal Guid? WorkItemId { get; set; }

    /// <summary>
    ///   After the work item is created, this will be set to the ID of the work item version
    /// </summary>
    internal Guid? WorkItemVersionId { get; set; }
}
