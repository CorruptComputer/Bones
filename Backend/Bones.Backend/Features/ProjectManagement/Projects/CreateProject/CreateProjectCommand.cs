using Bones.Database.DbSets.AccountManagement;

namespace Bones.Backend.Features.ProjectManagement.Projects.CreateProject;

/// <summary>
///     DB Command for creating a Project.
/// </summary>
/// <param name="Name">Name of the project</param>
/// <param name="RequestingUser">The user requesting this project be created</param>
/// <param name="OrganizationId">Optionally, the organization this project should belong to.</param>
public record CreateProjectCommand(string Name, BonesUser RequestingUser, Guid? OrganizationId = null) : IRequest<CommandResponse>;
