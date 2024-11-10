using Bones.Backend.Features.ProjectManagement.Projects.UserHasProjectPermission;
using Bones.Database.DbSets.ProjectManagement;
using Bones.Database.Operations.ProjectManagement.Projects.GetProjectByIdDb;
using Bones.Shared.Consts;

namespace Bones.Backend.Features.ProjectManagement.Projects.GetProjectById;

internal sealed class GetProjectByIdHandler(ISender sender) : IRequestHandler<GetProjectByIdQuery, QueryResponse<Project>>
{
    public async Task<QueryResponse<Project>> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        const string perm = BonesClaimTypes.Role.Project.VIEW_PROJECT;
        bool? hasPermission =
            await sender.Send(new UserHasProjectPermissionQuery(request.ProjectId, request.RequestingUser, perm), cancellationToken);

        if (hasPermission is not true)
        {
            return QueryResponse<Project>.Forbid();
        }

        Project? project = await sender.Send(new GetProjectByIdDbQuery(request.ProjectId), cancellationToken);

        if (project is null)
        {
            return QueryResponse<Project>.Fail("Project not found");
        }

        return QueryResponse<Project>.Pass(project);
    }
}