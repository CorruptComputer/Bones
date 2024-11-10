using Bones.Backend.Features.ProjectManagement.Projects.GetProjectsByOwner;
using Bones.Database.DbSets.ProjectManagement;
using Bones.Database.Operations.ProjectManagement.Projects.GetProjectsByOwnerDb;
using Bones.Shared.Backend.Enums;

namespace Bones.Backend.Features.ProjectManagement.Projects.GetProjectsUserCanAccess;

internal sealed class GetProjectsUserCanAccessHandler(ISender sender) : IRequestHandler<GetProjectsUserCanAccessQuery, QueryResponse<Dictionary<Guid, string>>>
{
    public async Task<QueryResponse<Dictionary<Guid, string>>> Handle(GetProjectsUserCanAccessQuery request, CancellationToken cancellationToken)
    {
        List<Project>? projects = await sender.Send(new GetProjectsByOwnerDbQuery(OwnershipType.User, request.RequestingUser.Id), cancellationToken);

        if (projects is null)
        {
            return QueryResponse<Dictionary<Guid, string>>.Fail("DB failed :(");
        }

        Dictionary<Guid, string> projectsUserCanAccess = projects.ToDictionary(project => project.Id, project => project.Name);

        // TODO: add projects from organizations

        return projectsUserCanAccess;
    }
}