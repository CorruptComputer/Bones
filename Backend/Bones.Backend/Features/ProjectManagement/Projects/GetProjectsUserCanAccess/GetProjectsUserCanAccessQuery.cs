using Bones.Database.DbSets.AccountManagement;

namespace Bones.Backend.Features.ProjectManagement.Projects.GetProjectsUserCanAccess;

/// <summary>
/// 
/// </summary>
/// <param name="RequestingUser"></param>
public sealed record GetProjectsUserCanAccessQuery(BonesUser RequestingUser) : IRequest<QueryResponse<Dictionary<Guid, string>>>;