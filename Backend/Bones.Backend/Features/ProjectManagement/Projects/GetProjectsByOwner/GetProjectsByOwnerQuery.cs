using Bones.Database.DbSets.AccountManagement;
using Bones.Shared.Backend.Enums;

namespace Bones.Backend.Features.ProjectManagement.Projects.GetProjectsByOwner;

/// <summary>
/// 
/// </summary>
/// <param name="OwnerType"></param>
/// <param name="OwnerId"></param>
/// <param name="RequestingUser"></param>
public sealed record GetProjectsByOwnerQuery(OwnershipType OwnerType, Guid OwnerId, BonesUser RequestingUser) : IRequest<QueryResponse<List<(Guid Id, string Name)>>>;