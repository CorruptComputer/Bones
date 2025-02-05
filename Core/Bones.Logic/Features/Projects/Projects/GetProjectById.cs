using Bones.Database.DbSets.AccountManagement;
using Bones.Database.Operations.ProjectManagement.Projects;
using Bones.Shared.Consts;

namespace Bones.Logic.Features.Projects.Projects;

/// <summary>
/// 
/// </summary>
/// <param name="ProjectId"></param>
/// <param name="RequestingUser"></param>
public sealed record GetProjectByIdQuery(Guid ProjectId, BonesUser RequestingUser) : IRequest<QueryResponse<Database.DbSets.ProjectManagement.Project>>;

internal sealed class GetProjectByIdQueryValidator : AbstractValidator<GetProjectByIdQuery>
{
    public GetProjectByIdQueryValidator()
    {
        RuleFor(x => x.RequestingUser).NotNull();
    }
}

internal sealed class GetProjectByIdHandler(ISender sender) : IRequestHandler<GetProjectByIdQuery, QueryResponse<Database.DbSets.ProjectManagement.Project>>
{
    public async Task<QueryResponse<Database.DbSets.ProjectManagement.Project>> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        const string perm = BonesClaimTypes.Role.Project.VIEW_PROJECT;
        bool? hasPermission =
            await sender.Send(new UserHasProjectPermissionQuery(request.ProjectId, request.RequestingUser, perm), cancellationToken);

        if (hasPermission is not true)
        {
            return QueryResponse<Database.DbSets.ProjectManagement.Project>.Forbid();
        }

        Database.DbSets.ProjectManagement.Project? project = await sender.Send(new GetProjectByIdDbQuery(request.ProjectId), cancellationToken);

        if (project is null)
        {
            return QueryResponse<Database.DbSets.ProjectManagement.Project>.Fail("Project not found");
        }

        return project;
    }
}