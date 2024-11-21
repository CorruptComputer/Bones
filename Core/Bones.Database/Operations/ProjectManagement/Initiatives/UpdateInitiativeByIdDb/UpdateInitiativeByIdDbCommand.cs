namespace Bones.Database.Operations.ProjectManagement.Initiatives.UpdateInitiativeByIdDb;

/// <summary>
///   DB Command for updating an initiative
/// </summary>
/// <param name="InitiativeId"></param>
/// <param name="NewName"></param>
public sealed record UpdateInitiativeByIdDbCommand(Guid InitiativeId, string NewName) : IRequest<CommandResponse>;
