namespace Bones.Database.Operations.ProjectManagement.Projects.UpdateProjectByIdDb;

/// <summary>
///     DB Command for updating a Project.
/// </summary>
/// <param name="ProjectId">Internal ID of the project</param>
/// <param name="Name">The new name of the project</param>
public record UpdateProjectByIdDbCommand(Guid ProjectId, string Name) : IRequest<CommandResponse>;