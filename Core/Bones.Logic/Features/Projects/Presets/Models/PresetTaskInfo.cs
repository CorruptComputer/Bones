using System.Diagnostics.CodeAnalysis;

namespace Bones.Logic.Features.Projects.Presets.Models;

internal record PresetTaskInfo
{
    internal required string Title { get; init; }

    internal required PresetLayoutInfo Layout { get; init; }

    internal required Dictionary<PresetFieldInfo, object?> Fields { get; init; }

    [MemberNotNullWhen(true, nameof(AssignmentState))]
    internal required bool ShouldAssignToCreator { get; init; }

    internal required string? AssignmentState { get; init; }

    [MemberNotNullWhen(true, nameof(TaskId))]
    [MemberNotNullWhen(true, nameof(TaskVersionId))]
    internal bool Created => TaskId.HasValue && TaskVersionId.HasValue;

    /// <summary>
    ///   After the task is created, this will be set to the ID of the task
    /// </summary>
    internal Guid? TaskId { get; set; }

    /// <summary>
    ///   After the task is created, this will be set to the ID of the task version
    /// </summary>
    internal Guid? TaskVersionId { get; set; }
}
