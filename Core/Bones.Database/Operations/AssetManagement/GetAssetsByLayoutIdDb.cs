using Bones.Database.DbSets.AssetManagement;
using Bones.Database.DbSets.Items;

namespace Bones.Database.Operations.AssetManagement;

/// <inheritdoc />
public sealed class GetAssetsByLayoutIdDb(BonesDbContext dbContext) : IRequestHandler<GetAssetsByLayoutIdDb.Query, QueryResponse<List<Asset>>>
{
    /// <summary>
    ///   DB Command for getting assets by layout ID.
    /// </summary>
    /// <param name="ItemLayoutId">ID of the layout</param>
    public sealed record Query(Guid ItemLayoutId) : IRequest<QueryResponse<List<Asset>>>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Query>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.ItemLayoutId).NotNull().NotEqual(Guid.Empty);
        }
    }

    /// <inheritdoc />
    public async Task<QueryResponse<List<Asset>>> Handle(Query request, CancellationToken cancellationToken)
    {
        List<Item> items = await dbContext.Items
            .Where(i => i.ItemLayoutId == request.ItemLayoutId)
            .ToListAsync(cancellationToken);

        IEnumerable<Guid> itemIds = items.Select(i => i.Id);
        List<Asset> assets = await dbContext.Assets.Where(a => itemIds.Contains(a.ItemId)).ToListAsync(cancellationToken);

        return assets;
    }
}