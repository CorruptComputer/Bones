using Bones.Database.DbSets.AccountManagement;
using Bones.Database.Operations.ProjectManagement.Projects;
using Bones.Shared.Backend.Enums;

namespace Bones.Logic.Features.Projects.Projects;

/// <inheritdoc />
public sealed class GetProjectsUserCanAccess(ISender sender) : IRequestHandler<GetProjectsUserCanAccess.Query, QueryResponse<Dictionary<Guid, string>>>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="RequestingUser"></param>
    public sealed record Query(BonesUser RequestingUser) : IRequest<QueryResponse<Dictionary<Guid, string>>>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Query>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.RequestingUser).NotNull();
        }
    }

    /// <inheritdoc />
    public async Task<QueryResponse<Dictionary<Guid, string>>> Handle(Query request, CancellationToken cancellationToken)
    {
        List<Database.DbSets.ProjectManagement.Project>? projects = await sender.Send(new GetProjectsByOwnerDb.Query(OwnershipType.User, request.RequestingUser.Id), cancellationToken);

        if (projects is null)
        {
            return QueryResponse<Dictionary<Guid, string>>.Fail("DB failed :(");
        }

        Dictionary<Guid, string> projectsUserCanAccess = projects.ToDictionary(project => project.Id, project => project.Name);

        // TODO: add projects from organizations

        return projectsUserCanAccess;
    }
}
