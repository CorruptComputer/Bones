using Bones.Backend.Features.Projects.Projects.UserHasProjectPermission;
using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.OrganizationManagement;
using Bones.Database.DbSets.ProjectManagement;
using Bones.Database.Operations.OrganizationManagement.GetOrganizationByIdDb;
using Bones.Database.Operations.ProjectManagement.Initiatives;
using Bones.Database.Operations.ProjectManagement.Projects;
using Bones.Shared.Backend.Enums;
using Bones.Shared.Consts;

namespace Bones.Backend.Features.Projects.Initiatives;

/// <summary>
///   Backend Command for creating an Initiative.
/// </summary>
/// <param name="ProjectId">Internal ID of the project</param>
/// <param name="RequestingUser">The user requesting this</param>
public record GetInitiativesByProjectQuery(Guid ProjectId, BonesUser RequestingUser) : IRequest<QueryResponse<List<Initiative>>>;

internal sealed class GetInitiativesByProjectQueryValidator : AbstractValidator<GetInitiativesByProjectQuery>
{

}

internal sealed class GetInitiativesByProjectHandler(ISender sender) : IRequestHandler<GetInitiativesByProjectQuery, QueryResponse<List<Initiative>>>
{
    public async Task<QueryResponse<List<Initiative>>> Handle(GetInitiativesByProjectQuery request, CancellationToken cancellationToken)
    {
        const string perm = BonesClaimTypes.Role.Initiative.VIEW_INITIATIVE;
        bool? hasOrganizationPermission =
            await sender.Send(new UserHasProjectPermissionQuery(request.ProjectId, request.RequestingUser, perm), cancellationToken);

        if (hasOrganizationPermission != true)
        {
            return QueryResponse<List<Initiative>>.Forbid();
        }

        return await sender.Send(new GetInitiativesByProjectDbQuery(request.ProjectId), cancellationToken);
    }
}
