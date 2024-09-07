using Bones.Database.DbSets.AccountManagement;

namespace Bones.Backend.Features.OrganizationManagement.UserHasOrganizationPermission;

/// <summary>
///   Checks if the user has permission to do the specified action in the organization.
/// </summary>
/// <param name="OrganizationId"></param>
/// <param name="User"></param>
/// <param name="Claim"></param>
public sealed record UserHasOrganizationPermissionQuery(Guid OrganizationId, BonesUser User, string Claim) : IRequest<QueryResponse<bool>>;