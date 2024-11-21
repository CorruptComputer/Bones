using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.OrganizationManagement;
using Bones.Database.DbSets.ProjectManagement;
using Bones.Database.Operations.OrganizationManagement.GetOrganizationByIdDb;
using Bones.Database.Operations.ProjectManagement.Projects;
using Bones.Shared.Backend.Enums;
using Bones.Shared.Consts;

namespace Bones.Database.Operations.ProjectManagement.Initiatives;

/// <summary>
///   DB Query for getting the initiatives under a project
/// </summary>
/// <param name="ProjectId">Internal ID of the project</param>
public record GetInitiativesByProjectDbQuery(Guid ProjectId) : IRequest<QueryResponse<List<Initiative>>>;

internal sealed class GetInitiativesByProjectQueryDbValidator : AbstractValidator<GetInitiativesByProjectDbQuery>
{

}

internal sealed class GetInitiativesByProjectDbHandler(BonesDbContext dbContext) : IRequestHandler<GetInitiativesByProjectDbQuery, QueryResponse<List<Initiative>>>
{
    public async Task<QueryResponse<List<Initiative>>> Handle(GetInitiativesByProjectDbQuery request, CancellationToken cancellationToken)
    {
        return await dbContext.Initiatives.Where(i => i.Project.Id == request.ProjectId).ToListAsync(cancellationToken);
    }
}