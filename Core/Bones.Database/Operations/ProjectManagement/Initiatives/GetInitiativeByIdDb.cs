using Bones.Database.DbSets.ProjectManagement;

namespace Bones.Database.Operations.ProjectManagement.Initiatives;

/// <summary>
///   DB Query for getting an initiative
/// </summary>
/// <param name="InitiativeId">Internal ID of the initiative</param>
public record GetInitiativesByIdDbQuery(Guid InitiativeId) : IRequest<QueryResponse<Initiative>>;

internal sealed class GetInitiativesByIdQueryDbValidator : AbstractValidator<GetInitiativesByIdDbQuery>
{

}

internal sealed class GetInitiativesByIdDbHandler(BonesDbContext dbContext) : IRequestHandler<GetInitiativesByIdDbQuery, QueryResponse<Initiative>>
{
    public async Task<QueryResponse<Initiative>> Handle(GetInitiativesByIdDbQuery request, CancellationToken cancellationToken)
    {
        return await dbContext.Initiatives
            .Include(i => i.Queues)
            .Include(i => i.Project)
            .FirstOrDefaultAsync(i => i.Id == request.InitiativeId, cancellationToken);
    }
}