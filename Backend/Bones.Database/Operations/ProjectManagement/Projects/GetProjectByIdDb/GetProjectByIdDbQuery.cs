using Bones.Database.DbSets.ProjectManagement;

namespace Bones.Database.Operations.ProjectManagement.Projects.GetProjectByIdDb;

/// <summary>
///   DB Query to get the project by its ID.
/// </summary>
/// <param name="ProjectId">The Project ID</param>
public sealed record GetProjectByIdDbQuery(Guid ProjectId) : IRequest<QueryResponse<Project>>;
