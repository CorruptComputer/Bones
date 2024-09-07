namespace Bones.Database.Operations.ProjectManagement.Tags.CreateTagDb;

/// <summary>
///     DB Command for creating a Project.
/// </summary>
/// <param name="Name">Name of the project</param>
public sealed record CreateTagDbCommand(string Name) : IRequest<CommandResponse>;