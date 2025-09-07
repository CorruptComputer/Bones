using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.GenericItems;
using Bones.Database.Operations.GenericItem;
using Bones.Logic.Features.Projects;
using Bones.Shared.Consts;

namespace Bones.Logic.Features.GenericItem;

/// <inheritdoc />
public sealed class GetItemLayoutById(ISender sender) : IRequestHandler<GetItemLayoutById.Query, QueryResponse<GenericItemLayout?>>
{
    /// <summary>
    ///   Query for getting an item layout by its internal ID
    /// </summary>
    /// <param name="ItemLayoutId">Internal ID of the item layout</param>
    /// <param name="RequestingUser">The user requesting this</param>
    public record Query(Guid ItemLayoutId, BonesUser RequestingUser) : IRequest<QueryResponse<GenericItemLayout?>>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Query>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.ItemLayoutId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.RequestingUser).NotNull();
        }
    }

    /// <inheritdoc />
    public async Task<QueryResponse<GenericItemLayout?>> Handle(Query request, CancellationToken cancellationToken)
    {
        GenericItemLayout? itemField = await sender.Send(new GetItemLayoutByIdDb.Query(request.ItemLayoutId), cancellationToken);

        if (itemField == null)
        {
            return QueryResponse<GenericItemLayout?>.Fail("ItemField not found");
        }

        const string perm = BonesClaimTypes.Role.Project.VIEW_PROJECT;
        bool? hasProjectPermission =
            await sender.Send(new UserHasProjectPermission.Query(itemField.Project.Id, request.RequestingUser, perm), cancellationToken);

        if (hasProjectPermission != true)
        {
            return QueryResponse<GenericItemLayout?>.Forbid();
        }

        return itemField;
    }
}