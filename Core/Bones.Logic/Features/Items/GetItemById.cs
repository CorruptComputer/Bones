using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.Items;
using Bones.Database.Operations.Items;
using Bones.Logic.Features.Projects;
using Bones.Shared.Consts;

namespace Bones.Logic.Features.Items;

/// <inheritdoc />
public sealed class GetItemById(ISender sender) : IRequestHandler<GetItemById.Query, QueryResponse<Item?>>
{
    /// <summary>
    ///   Query for getting an item layout by its internal ID
    /// </summary>
    /// <param name="ItemId">Internal ID of the item</param>
    /// <param name="RequestingUser">The user requesting this</param>
    /// <param name="IncludeProject">Whether to include the related project</param>
    /// <param name="IncludeVersions">Whether to include the item versions</param>
    public record Query(Guid ItemId, BonesUser RequestingUser, bool IncludeProject = false, bool IncludeVersions = false) : IRequest<QueryResponse<Item?>>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Query>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.ItemId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.RequestingUser).NotNull();
        }
    }

    /// <inheritdoc />
    public async Task<QueryResponse<Item?>> Handle(Query request, CancellationToken cancellationToken)
    {
        Item? item = await sender.Send(new GetItemByIdDb.Query(request.ItemId, request.IncludeProject, request.IncludeVersions), cancellationToken);
        if (item == null)
        {
            return QueryResponse<Item?>.Fail("Item not found");
        }

        const string perm = BonesClaimTypes.Role.Project.VIEW_PROJECT;
        bool? hasProjectPermission =
            await sender.Send(new UserHasProjectPermission.Query(item.ProjectId, request.RequestingUser, perm), cancellationToken);

        if (hasProjectPermission != true)
        {
            return QueryResponse<Item?>.Forbid();
        }

        return item;
    }
}