using Bones.Database.DbSets.AccountManagement;
using Bones.Database.Operations.ProjectManagement.Projects;
using Bones.Shared.Consts;

namespace Bones.Logic.Features.Projects;

/// <inheritdoc />
public sealed class GetProjectById(ISender sender) : IRequestHandler<GetProjectById.Query, QueryResponse<Database.DbSets.ProjectManagement.Project>>
{
    /// <summary>
    ///   Query to get a project by its ID
    /// </summary>
    /// <param name="ProjectId"></param>
    /// <param name="RequestingUser"></param>
    public sealed record Query(Guid ProjectId, BonesUser RequestingUser) : IRequest<QueryResponse<Database.DbSets.ProjectManagement.Project>>;

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
    public async Task<QueryResponse<Database.DbSets.ProjectManagement.Project>> Handle(Query request, CancellationToken cancellationToken)
    {
        const string perm = BonesClaimTypes.Role.Project.VIEW_PROJECT;
        bool? hasPermission =
            await sender.Send(new UserHasProjectPermission.Query(request.ProjectId, request.RequestingUser, perm), cancellationToken);

        if (hasPermission is not true)
        {
            return QueryResponse<Database.DbSets.ProjectManagement.Project>.Forbid();
        }

        Database.DbSets.ProjectManagement.Project? project = await sender.Send(new GetProjectByIdDb.Query(request.ProjectId), cancellationToken);

        if (project is null)
        {
            return QueryResponse<Database.DbSets.ProjectManagement.Project>.Fail("Project not found");
        }

        return project;
    }
}