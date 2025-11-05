using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.ProjectManagement;
using Bones.Database.Operations.ProjectManagement.Initiatives;
using Bones.Logic.Features.Projects;
using Bones.Shared.Consts;

namespace Bones.Logic.Features.Initiatives;

/// <inheritdoc />
public sealed class GetInitiativesByProject(ISender sender) : IRequestHandler<GetInitiativesByProject.Query, QueryResponse<List<Initiative>>>
{
    /// <summary>
    ///   Backend query for getting initiatives that belong to a project.
    /// </summary>
    /// <param name="ProjectId">Internal ID of the project</param>
    /// <param name="RequestingUser">The user requesting this</param>
    public record Query(Guid ProjectId, BonesUser RequestingUser) : IRequest<QueryResponse<List<Initiative>>>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Query>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.ProjectId).NotEmpty();
            RuleFor(x => x.RequestingUser).NotNull();
        }
    }

    /// <inheritdoc />
    public async Task<QueryResponse<List<Initiative>>> Handle(Query request, CancellationToken cancellationToken)
    {
        const string perm = BonesClaimTypes.Role.Initiative.VIEW_INITIATIVE;
        bool? hasOrganizationPermission =
            await sender.Send(new UserHasProjectPermission.Query(request.ProjectId, request.RequestingUser, perm), cancellationToken);

        if (hasOrganizationPermission != true)
        {
            return QueryResponse<List<Initiative>>.Forbid();
        }

        return await sender.Send(new GetInitiativesByProjectDb.Query(request.ProjectId), cancellationToken);
    }
}
