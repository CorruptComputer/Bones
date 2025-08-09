using System.Diagnostics.CodeAnalysis;

namespace Bones.Logic.Features.Projects.Presets.Models;

internal record PresetInitiativeInfo
{
    internal Dictionary<string, PresetWorkItemQueueInfo> WorkItemQueues { get; init; } = [];

    [MemberNotNullWhen(true, nameof(InitiativeId))]
    internal bool Created => InitiativeId.HasValue;

    /// <summary>
    ///   After the initiative is created, this will be set to the ID of the initiative
    /// </summary>
    internal Guid? InitiativeId { get; set; }
}