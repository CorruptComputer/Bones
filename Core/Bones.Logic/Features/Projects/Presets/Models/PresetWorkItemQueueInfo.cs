using System.Diagnostics.CodeAnalysis;

namespace Bones.Logic.Features.Projects.Presets.Models;

internal record PresetWorkItemQueueInfo
{
    [MemberNotNullWhen(true, nameof(WorkItemQueueId))]
    internal bool Created => WorkItemQueueId.HasValue;

    /// <summary>
    ///   After the work item queue is created, this will be set to the ID of the work item queue
    /// </summary>
    internal Guid? WorkItemQueueId { get; set; }
}
