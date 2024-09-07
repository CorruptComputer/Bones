using System.Security.Claims;

namespace Bones.Backend.Features.ProjectManagement.Projects.CreateProject;

/// <summary>
///     DB Command for creating a Project.
/// </summary>
/// <param name="Name">Name of the project</param>
public record CreateProjectCommand(string Name, ClaimsPrincipal RequestingUser, Guid? OrganizationId = null) : IRequest<CommandResponse>;
