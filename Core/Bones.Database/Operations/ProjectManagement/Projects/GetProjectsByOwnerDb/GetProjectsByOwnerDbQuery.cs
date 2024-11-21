using Bones.Database.DbSets.ProjectManagement;
using Bones.Shared.Backend.Enums;

namespace Bones.Database.Operations.ProjectManagement.Projects.GetProjectsByOwnerDb;

/// <summary>
///   DB Query to get projects by its owner
/// </summary>
/// <param name="OwnerType">The owner type</param>
/// <param name="OwnerId">The owner id</param>
public sealed record GetProjectsByOwnerDbQuery(OwnershipType OwnerType, Guid OwnerId) : IRequest<QueryResponse<List<Project>>>;
