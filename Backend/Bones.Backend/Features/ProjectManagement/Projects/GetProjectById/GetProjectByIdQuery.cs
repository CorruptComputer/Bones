using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.ProjectManagement;

namespace Bones.Backend.Features.ProjectManagement.Projects.GetProjectById;

/// <summary>
/// 
/// </summary>
/// <param name="ProjectId"></param>
/// <param name="RequestingUser"></param>
public sealed record GetProjectByIdQuery(Guid ProjectId, BonesUser RequestingUser) : IRequest<QueryResponse<Project>>;