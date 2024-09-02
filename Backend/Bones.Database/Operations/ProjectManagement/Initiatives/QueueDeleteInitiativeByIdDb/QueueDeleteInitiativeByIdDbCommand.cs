namespace Bones.Database.Operations.ProjectManagement.Initiatives.QueueDeleteInitiativeByIdDb;

/// <summary>
///   DB Command for queueing an initiative for deletion
/// </summary>
/// <param name="InitiativeId"></param>
public sealed record QueueDeleteInitiativeByIdDbCommand(Guid InitiativeId) : IRequest<CommandResponse>;
