namespace Bones.Database.Operations.ProjectManagement.Projects.QueueDeleteProjectByIdDb;

/// <summary>
///     DB Command for deleting a Project.
/// </summary>
/// <param name="ProjectId">Internal ID of the project</param>
public sealed record QueueDeleteProjectByIdDbCommand(Guid ProjectId) : IRequest<CommandResponse>;