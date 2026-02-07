using Bones.Database.DbSets.Accounts;
using Bones.Database.DbSets.Items.Layouts;
using Bones.Database.Operations.Items.Layouts;
using Bones.Logic.Features.Projects;
using Bones.Shared.Consts;

namespace Bones.Logic.Features.Items;

/// <inheritdoc />
public sealed class GetItemLayoutById(ISender sender) : IRequestHandler<GetItemLayoutById.Query, QueryResponse<ItemLayout?>>
{
    /// <summary>
    ///   Query for getting an item layout by its internal ID
    /// </summary>
    /// <param name="ItemLayoutId">Internal ID of the item layout</param>
    /// <param name="RequestingUser">The user requesting this</param>
    public record Query(Guid ItemLayoutId, BonesUser RequestingUser) : IRequest<QueryResponse<ItemLayout?>>;

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
    public async Task<QueryResponse<ItemLayout?>> Handle(Query request, CancellationToken cancellationToken)
    {
        ItemLayout? itemField = await sender.Send(new GetItemLayoutByIdDb.Query(request.ItemLayoutId), cancellationToken);

        if (itemField == null)
        {
            return QueryResponse<ItemLayout?>.Fail("ItemField not found");
        }

        const string perm = BonesClaimTypes.Role.Project.VIEW_PROJECT;
        bool? hasProjectPermission =
            await sender.Send(new UserHasProjectPermission.Query(itemField.ProjectId, request.RequestingUser, perm), cancellationToken);

        if (hasProjectPermission != true)
        {
            return QueryResponse<ItemLayout?>.Forbid();
        }

        return itemField;
    }
}