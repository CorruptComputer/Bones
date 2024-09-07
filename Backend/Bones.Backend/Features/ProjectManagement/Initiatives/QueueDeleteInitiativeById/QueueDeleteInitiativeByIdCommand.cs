using Bones.Database.DbSets.AccountManagement;

namespace Bones.Backend.Features.ProjectManagement.Initiatives.QueueDeleteInitiativeById;

/// <summary>
///   Queues the deletion of the initiative
/// </summary>
/// <param name="InitiativeId"></param>
/// <param name="RequestingUser"></param>
public sealed record QueueDeleteInitiativeByIdCommand(Guid InitiativeId, BonesUser RequestingUser) : IRequest<CommandResponse>;