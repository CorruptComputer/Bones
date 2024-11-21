namespace Bones.Database.Operations.ProjectManagement.Initiatives.CreateInitiativeDb;

/// <summary>
///     DB Command for creating an Initiative.
/// </summary>
/// <param name="Name">Name of the initiative</param>
/// <param name="ProjectId">Internal ID of the project</param>
public sealed record CreateInitiativeDbCommand(string Name, Guid ProjectId) : IRequest<CommandResponse>;