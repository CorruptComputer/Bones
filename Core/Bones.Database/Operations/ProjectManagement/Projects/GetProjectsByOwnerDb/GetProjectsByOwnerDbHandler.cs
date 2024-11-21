using Bones.Database.DbSets.ProjectManagement;
using Bones.Shared.Backend.Enums;

namespace Bones.Database.Operations.ProjectManagement.Projects.GetProjectsByOwnerDb;

internal sealed class GetProjectsByOwnerDbHandler(BonesDbContext dbContext) : IRequestHandler<GetProjectsByOwnerDbQuery, QueryResponse<List<Project>>>
{
    public async Task<QueryResponse<List<Project>>> Handle(GetProjectsByOwnerDbQuery request, CancellationToken cancellationToken)
    {
        if (request.OwnerType == OwnershipType.User)
        {
            List<Project> projects = await dbContext.Projects.Where(p => p.OwningUser != null && p.OwningUser.Id == request.OwnerId).ToListAsync(cancellationToken);
            return QueryResponse<List<Project>>.Pass(projects);
        }

        return QueryResponse<List<Project>>.Fail("org not implemented");
    }
}