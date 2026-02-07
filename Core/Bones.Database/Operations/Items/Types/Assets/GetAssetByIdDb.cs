using Bones.Database.DbSets.Items.Types;

namespace Bones.Database.Operations.Items.Types.Assets;

/// <inheritdoc />
public sealed class GetAssetByIdDb(BonesDbContext dbContext) : IRequestHandler<GetAssetByIdDb.Query, QueryResponse<Asset?>>
{
    /// <summary>
    ///   DB Command for getting an Asset by id.
    /// </summary>
    /// <param name="AssetId">ID of the item</param>
    /// <param name="IncludeProject">Whether to include the related Project in the query</param>
    /// <param name="IncludeItem">Whether to include the related Item in the query</param>
    public sealed record Query(Guid AssetId, bool IncludeProject = false, bool IncludeItem = false) : IRequest<QueryResponse<Asset?>>;

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
    public async Task<QueryResponse<Asset?>> Handle(Query request, CancellationToken cancellationToken)
    {
        IQueryable<Asset> assetQuery = dbContext.Assets;

        if (request.IncludeProject)
        {
            assetQuery = assetQuery.Include(a => a.Project);
        }

        if (request.IncludeItem)
        {
            assetQuery = assetQuery.Include(a => a.Item);
        }

        Asset? asset = await assetQuery.AsNoTracking()
                                       .FirstOrDefaultAsync(i => i.Id == request.AssetId, cancellationToken);

        return asset;
    }
}