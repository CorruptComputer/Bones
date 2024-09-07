using System.Security.Claims;

namespace Bones.Backend.Features.ProjectManagement.Initiatives.CreateInitiative;

/// <summary>
///   Backend Command for creating an Initiative.
/// </summary>
/// <param name="Name">Name of the initiative</param>
/// <param name="ProjectId">Internal ID of the project</param>
/// <param name="RequestingUser">The claims principal for the user requesting this</param>
public record CreateInitiativeCommand(string Name, Guid ProjectId, ClaimsPrincipal RequestingUser) : IRequest<CommandResponse>;