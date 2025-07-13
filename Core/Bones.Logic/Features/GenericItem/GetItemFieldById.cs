using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.GenericItems;
using Bones.Database.Operations.GenericItem;
using Bones.Logic.Features.Projects;
using Bones.Shared.Consts;

namespace Bones.Logic.Features.GenericItem;

/// <inheritdoc />
public sealed class GetItemFieldById(ISender sender) : IRequestHandler<GetItemFieldById.Query, QueryResponse<GenericItemField?>>
{
    /// <summary>
    ///     Query for getting an item field by its internal ID
    /// </summary>
    /// <param name="ItemFieldId">Internal ID of the item field</param>
    /// <param name="RequestingUser">The user requesting this</param>
    public record Query(Guid ItemFieldId, BonesUser RequestingUser) : IRequest<QueryResponse<GenericItemField?>>;

    /// <inheritdoc />
    public sealed class GetItemFieldByIdQueryValidator : AbstractValidator<Query>
    {
        /// <inheritdoc />
        public GetItemFieldByIdQueryValidator()
        {
            RuleFor(x => x.ItemFieldId).NotNull().NotEqual(Guid.Empty);
        }
    }

    /// <inheritdoc />
    public async Task<QueryResponse<GenericItemField?>> Handle(Query request, CancellationToken cancellationToken)
    {
        GenericItemField? itemField = await sender.Send(new GetItemFieldByIdDb.Query(request.ItemFieldId), cancellationToken);

        if (itemField == null)
        {
            return QueryResponse<GenericItemField?>.Fail("ItemField not found");
        }

        const string perm = BonesClaimTypes.Role.Project.VIEW_PROJECT;
        bool? hasProjectPermission =
            await sender.Send(new UserHasProjectPermission.Query(itemField.Project.Id, request.RequestingUser, perm), cancellationToken);

        if (hasProjectPermission != true)
        {
            return QueryResponse<GenericItemField?>.Forbid();
        }

        return itemField;
    }
}