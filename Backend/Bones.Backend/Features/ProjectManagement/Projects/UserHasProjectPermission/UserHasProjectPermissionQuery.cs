using Bones.Database.DbSets.AccountManagement;

namespace Bones.Backend.Features.ProjectManagement.Projects.UserHasProjectPermission;

/// <summary>
///   Checks if the user has permission to do the specified action in the project.
/// </summary>
/// <param name="ProjectId"></param>
/// <param name="User"></param>
/// <param name="Claim"></param>
public sealed record UserHasProjectPermissionQuery(Guid ProjectId, BonesUser User, string Claim) : IRequest<QueryResponse<bool>>;