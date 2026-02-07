using Bones.Database.DbSets.Accounts;
using Bones.Logic.Features.Tasks.TaskQueues;

namespace Bones.Api.Models.TaskQueues;

/// <summary>
///   Response for the CreateTaskQueueInInitiativeAsync endpoint
/// </summary>
[JsonSerializable(typeof(CreateTaskQueueInInitiativeRequest))]
public record CreateTaskQueueInInitiativeRequest
{
    /// <summary>
    ///   The name of the queue
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    ///   The ID of the initiative to create the queue in
    /// </summary>
    public required Guid InitiativeId { get; init; }

    internal CreateTaskQueue.Command ToInternal(BonesUser user)
    {
        return new(Name, InitiativeId, user);
    }
}
