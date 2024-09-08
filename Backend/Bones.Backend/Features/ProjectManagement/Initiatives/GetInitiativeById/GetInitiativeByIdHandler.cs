using Bones.Database.DbSets.ProjectManagement;

namespace Bones.Backend.Features.ProjectManagement.Initiatives.GetInitiativeById;

internal sealed class GetInitiativeByIdHandler : IRequestHandler<GetInitiativeByIdQuery, QueryResponse<Initiative>>
{
    public Task<QueryResponse<Initiative>> Handle(GetInitiativeByIdQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}