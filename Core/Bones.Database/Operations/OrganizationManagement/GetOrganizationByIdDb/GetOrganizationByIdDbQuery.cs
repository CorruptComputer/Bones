using Bones.Database.DbSets.OrganizationManagement;

namespace Bones.Database.Operations.OrganizationManagement.GetOrganizationByIdDb;

/// <summary>
///   DB Query to get the specified organization.
/// </summary>
/// <param name="OrganizationId"></param>
public sealed record GetOrganizationByIdDbQuery(Guid OrganizationId) : IRequest<QueryResponse<BonesOrganization>>;