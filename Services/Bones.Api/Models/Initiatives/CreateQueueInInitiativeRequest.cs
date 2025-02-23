using Bones.Database.DbSets.AccountManagement;
using Bones.Logic.Features.WorkItems.Queue;

namespace Bones.Api.Models.Initiatives;

/// <summary>
///   Response for the CreateQueueInInitiativeAsync endpoint
/// </summary>
[JsonSerializable(typeof(CreateQueueInInitiativeRequest))]
public record CreateQueueInInitiativeRequest
{
    /// <summary>
    ///   The name of the queue
    /// </summary>
    public required string Name { get; init; }

    internal CreateWorkItemQueue.Command ToInternal(Guid initiativeId, BonesUser user)
    {
        return new(Name, initiativeId, user);
    }
}
