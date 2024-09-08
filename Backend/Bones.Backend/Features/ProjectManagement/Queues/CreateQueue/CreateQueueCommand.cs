using Bones.Database.DbSets.AccountManagement;

namespace Bones.Backend.Features.ProjectManagement.Queues.CreateQueue;

/// <summary>
///     Command for creating a Queue.
/// </summary>
/// <param name="Name">Name of the queue</param>
/// <param name="InitiativeId">Internal ID of the initiative</param>
/// <param name="RequestingUser"></param>
public sealed record CreateQueueCommand(string Name, Guid InitiativeId, BonesUser RequestingUser) : IRequest<CommandResponse>;