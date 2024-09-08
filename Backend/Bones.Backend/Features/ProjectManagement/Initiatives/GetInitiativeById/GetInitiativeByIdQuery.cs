using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.ProjectManagement;

namespace Bones.Backend.Features.ProjectManagement.Initiatives.GetInitiativeById;

/// <summary>
///   Gets the initiative by ID
/// </summary>
/// <param name="InitiativeId"></param>
/// <param name="RequestingUser"></param>
public sealed record GetInitiativeByIdQuery(Guid InitiativeId, BonesUser RequestingUser) : IRequest<QueryResponse<Initiative>>;