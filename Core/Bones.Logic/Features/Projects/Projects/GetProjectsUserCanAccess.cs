using Bones.Database.DbSets.AccountManagement;
using Bones.Database.Operations.ProjectManagement.Projects.GetProjectsByOwnerDb;
using Bones.Shared.Backend.Enums;

namespace Bones.Logic.Features.Projects.Projects;

/// <summary>
/// 
/// </summary>
/// <param name="RequestingUser"></param>
public sealed record GetProjectsUserCanAccessQuery(BonesUser RequestingUser) : IRequest<QueryResponse<Dictionary<Guid, string>>>;

internal sealed class GetProjectsUserCanAccessQueryValidator : AbstractValidator<GetProjectsUserCanAccessQuery>
{
    public GetProjectsUserCanAccessQueryValidator()
    {
        RuleFor(x => x.RequestingUser).NotNull();
    }
}

internal sealed class GetProjectsUserCanAccessHandler(ISender sender) : IRequestHandler<GetProjectsUserCanAccessQuery, QueryResponse<Dictionary<Guid, string>>>
{
    public async Task<QueryResponse<Dictionary<Guid, string>>> Handle(GetProjectsUserCanAccessQuery request, CancellationToken cancellationToken)
    {
        List<Database.DbSets.ProjectManagement.Project>? projects = await sender.Send(new GetProjectsByOwnerDbQuery(OwnershipType.User, request.RequestingUser.Id), cancellationToken);

        if (projects is null)
        {
            return QueryResponse<Dictionary<Guid, string>>.Fail("DB failed :(");
        }

        Dictionary<Guid, string> projectsUserCanAccess = projects.ToDictionary(project => project.Id, project => project.Name);

        // TODO: add projects from organizations

        return projectsUserCanAccess;
    }
}
