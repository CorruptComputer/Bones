using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.GenericItems;
using Bones.Database.Operations.GenericItem;
using Bones.Logic.Features.Projects.Projects;
using Bones.Shared.Consts;

namespace Bones.Logic.Features.GenericItem;

/// <summary>
///     Query for getting the item layouts in a project
/// </summary>
/// <param name="ProjectId">Internal ID of the project</param>
/// <param name="RequestingUser">The user requesting this</param>
public record GetItemLayoutsByProjectQuery(Guid ProjectId, BonesUser RequestingUser) : IRequest<QueryResponse<List<GenericItemLayout>>>;

internal sealed class GetItemLayoutsByProjectQueryValidator : AbstractValidator<GetItemLayoutsByProjectQuery>
{

}

internal sealed class GetItemLayoutsByProjectHandler(ISender sender) : IRequestHandler<GetItemLayoutsByProjectQuery, QueryResponse<List<GenericItemLayout>>>
{
    public async Task<QueryResponse<List<GenericItemLayout>>> Handle(GetItemLayoutsByProjectQuery request, CancellationToken cancellationToken)
    {
        const string perm = BonesClaimTypes.Role.Project.VIEW_PROJECT;
        bool? hasProjectPermission =
            await sender.Send(new UserHasProjectPermissionQuery(request.ProjectId, request.RequestingUser, perm), cancellationToken);

        if (hasProjectPermission != true)
        {
            return QueryResponse<List<GenericItemLayout>>.Forbid();
        }

        return await sender.Send(new GetItemLayoutsByProjectDbQuery(request.ProjectId), cancellationToken);
    }
}
