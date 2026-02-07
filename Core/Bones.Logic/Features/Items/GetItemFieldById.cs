using Bones.Database.DbSets.Accounts;
using Bones.Database.DbSets.Items.Fields;
using Bones.Database.Operations.Items.Fields;
using Bones.Logic.Features.Projects;
using Bones.Shared.Consts;

namespace Bones.Logic.Features.Items;

/// <inheritdoc />
public sealed class GetItemFieldById(ISender sender) : IRequestHandler<GetItemFieldById.Query, QueryResponse<ItemField?>>
{
    /// <summary>
    ///   Query for getting an item field by its internal ID
    /// </summary>
    /// <param name="ItemFieldId">Internal ID of the item field</param>
    /// <param name="RequestingUser">The user requesting this</param>
    public record Query(Guid ItemFieldId, BonesUser RequestingUser) : IRequest<QueryResponse<ItemField?>>;

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
    public async Task<QueryResponse<ItemField?>> Handle(Query request, CancellationToken cancellationToken)
    {
        ItemField? itemField = await sender.Send(new GetItemFieldByIdDb.Query(request.ItemFieldId), cancellationToken);

        if (itemField == null)
        {
            return QueryResponse<ItemField?>.Fail("ItemField not found");
        }

        const string perm = BonesClaimTypes.Role.Project.VIEW_PROJECT;
        bool? hasProjectPermission =
            await sender.Send(new UserHasProjectPermission.Query(itemField.ProjectId, request.RequestingUser, perm), cancellationToken);

        if (hasProjectPermission != true)
        {
            return QueryResponse<ItemField?>.Forbid();
        }

        return itemField;
    }
}