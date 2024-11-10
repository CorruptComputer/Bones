using Bones.Database.DbSets.AccountManagement;

namespace Bones.Backend.Features.Projects.Projects.GetProjectById;

/// <summary>
/// 
/// </summary>
/// <param name="ProjectId"></param>
/// <param name="RequestingUser"></param>
public sealed record GetProjectByIdQuery(Guid ProjectId, BonesUser RequestingUser) : IRequest<QueryResponse<Database.DbSets.ProjectManagement.Project>>;