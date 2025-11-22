using System.Diagnostics.CodeAnalysis;

namespace Bones.Logic.Features.Projects.Presets.Models;

internal record PresetTaskQueueInfo
{
    [MemberNotNullWhen(true, nameof(TaskQueueId))]
    internal bool Created => TaskQueueId.HasValue;

    /// <summary>
    ///   After the task queue is created, this will be set to the ID of the task queue
    /// </summary>
    internal Guid? TaskQueueId { get; set; }
}
