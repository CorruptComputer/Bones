using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.GenericItems;
using Bones.Database.Operations.GenericItem;
using Bones.Logic.Features.Projects.Projects;
using Bones.Shared.Consts;

namespace Bones.Logic.Features.GenericItem;

/// <summary>
///     Query for getting an item field by its internal ID
/// </summary>
/// <param name="ItemFieldId">Internal ID of the item field</param>
/// <param name="RequestingUser">The user requesting this</param>
public record GetItemFieldByIdQuery(Guid ItemFieldId, BonesUser RequestingUser) : IRequest<QueryResponse<GenericItemField?>>;

internal sealed class GetItemFieldByIdQueryValidator : AbstractValidator<GetItemFieldByIdQuery>
{

}

internal sealed class GetItemFieldByIdHandler(ISender sender) : IRequestHandler<GetItemFieldByIdQuery, QueryResponse<GenericItemField?>>
{
    public async Task<QueryResponse<GenericItemField?>> Handle(GetItemFieldByIdQuery request, CancellationToken cancellationToken)
    {
        GenericItemField? itemField = await sender.Send(new GetItemFieldByIdDbQuery(request.ItemFieldId), cancellationToken);

        if (itemField == null)
        {
            return QueryResponse<GenericItemField?>.Fail("ItemField not found");
        }

        const string perm = BonesClaimTypes.Role.Project.VIEW_PROJECT;
        bool? hasProjectPermission =
            await sender.Send(new UserHasProjectPermissionQuery(itemField.ProjectId, request.RequestingUser, perm), cancellationToken);

        if (hasProjectPermission != true)
        {
            return QueryResponse<GenericItemField?>.Forbid();
        }

        return itemField;
    }
}