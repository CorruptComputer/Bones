using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.GenericItems;
using Bones.Database.Operations.GenericItem;
using Bones.Logic.Features.Projects.Projects;
using Bones.Shared.Consts;

namespace Bones.Logic.Features.GenericItem;

/// <summary>
///     Query for getting the item fields in a project
/// </summary>
/// <param name="ProjectId">Internal ID of the project</param>
/// <param name="RequestingUser">The user requesting this</param>
public record GetItemFieldsByProjectQuery(Guid ProjectId, BonesUser RequestingUser) : IRequest<QueryResponse<List<GenericItemField>>>;

internal sealed class GetItemFieldsByProjectQueryValidator : AbstractValidator<GetItemFieldsByProjectQuery>
{

}

internal sealed class GetItemFieldsByProjectHandler(ISender sender) : IRequestHandler<GetItemFieldsByProjectQuery, QueryResponse<List<GenericItemField>>>
{
    public async Task<QueryResponse<List<GenericItemField>>> Handle(GetItemFieldsByProjectQuery request, CancellationToken cancellationToken)
    {
        const string perm = BonesClaimTypes.Role.Project.VIEW_PROJECT;
        bool? hasProjectPermission =
            await sender.Send(new UserHasProjectPermissionQuery(request.ProjectId, request.RequestingUser, perm), cancellationToken);

        if (hasProjectPermission != true)
        {
            return QueryResponse<List<GenericItemField>>.Forbid();
        }

        return await sender.Send(new GetItemFieldsByProjectDbQuery(request.ProjectId), cancellationToken);
    }
}
