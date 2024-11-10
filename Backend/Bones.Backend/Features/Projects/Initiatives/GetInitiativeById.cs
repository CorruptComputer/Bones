using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.ProjectManagement;

namespace Bones.Backend.Features.Projects.Initiatives;

/// <summary>
///   Gets the initiative by ID
/// </summary>
/// <param name="InitiativeId"></param>
/// <param name="RequestingUser"></param>
public sealed record GetInitiativeByIdQuery(Guid InitiativeId, BonesUser RequestingUser) : IRequest<QueryResponse<Initiative>>;

internal sealed class GetInitiativeByIdQueryValidator
{

}

internal sealed class GetInitiativeByIdHandler : IRequestHandler<GetInitiativeByIdQuery, QueryResponse<Initiative>>
{
    public Task<QueryResponse<Initiative>> Handle(GetInitiativeByIdQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}