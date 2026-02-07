using Bones.Database.DbSets.Items;
using Bones.Database.DbSets.Items.Assignments;
using Bones.Database.DbSets.Items.Types;

namespace Bones.Database.Operations.Items.Types.Assets;

/// <inheritdoc />
public sealed class GetAssetCurrentAssigneesByIdDb(BonesDbContext dbContext) : IRequestHandler<GetAssetCurrentAssigneesByIdDb.Query, QueryResponse<List<ItemAssignee>>>
{
    /// <summary>
    ///   DB Command for getting an Asset by id.
    /// </summary>
    /// <param name="AssetId">ID of the item</param>
    /// <param name="IncludeAssigneeSlot">Whether to include the related AssigneeSlot in the query</param>
    /// <param name="IncludeAssignee">Whether to include the related BonesUser or BonesRole in the query</param>
    public sealed record Query(Guid AssetId, bool IncludeAssigneeSlot = false, bool IncludeAssignee = false) : IRequest<QueryResponse<List<ItemAssignee>>>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Query>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.AssetId).NotNull().NotEqual(Guid.Empty);
        }
    }

    /// <inheritdoc />
    public async Task<QueryResponse<List<ItemAssignee>>> Handle(Query request, CancellationToken cancellationToken)
    {
        Asset? asset = await dbContext.Assets.FirstOrDefaultAsync(i => i.Id == request.AssetId, cancellationToken);

        if (asset is null)
        {
            return QueryResponse<List<ItemAssignee>>.Fail("Asset not found.");
        }

        Item? item = await dbContext.Items
            .Include(i => i.Versions)
            .FirstOrDefaultAsync(i => i.Id == asset.ItemId, cancellationToken);

        if (item is null || item.Current is null)
        {
            return QueryResponse<List<ItemAssignee>>.Fail("Item not found.");
        }

        IQueryable<ItemVersion> versionQuery = dbContext.ItemVersions.Include(iv => iv.ItemAssignees);
        if (request.IncludeAssigneeSlot)
        {
            versionQuery = versionQuery.Include(iv => iv.ItemAssignees).ThenInclude(a => a.ItemAssignmentSlot);
        }

        if (request.IncludeAssignee)
        {
            versionQuery = versionQuery.Include(iv => iv.ItemAssignees).ThenInclude(a => a.AssignedUser)
                                       .Include(iv => iv.ItemAssignees).ThenInclude(a => a.AssignedRole);
        }

        ItemVersion? itemVersion = await versionQuery.FirstOrDefaultAsync(iv => iv.Id == item.Current.Id, cancellationToken);

        if (itemVersion is null)
        {
            return QueryResponse<List<ItemAssignee>>.Fail("Item version not found.");
        }

        return itemVersion.ItemAssignees;
    }
}