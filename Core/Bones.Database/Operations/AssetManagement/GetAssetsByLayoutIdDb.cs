using Bones.Database.DbSets.AssetManagement;

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
        List<Asset> assets = await dbContext.Assets
            .Include(a => a.Item).ThenInclude(i => i.Project)
            .Include(a => a.Item).ThenInclude(i => i.ItemLayout)
            .Include(a => a.Item).ThenInclude(i => i.Versions).ThenInclude(iv => iv.ItemLayoutVersion).ThenInclude(lv => lv.FieldLinks).ThenInclude(fl => fl.FieldVersion).ThenInclude(fv => fv.PossibleValues)
            .Include(a => a.Item).ThenInclude(i => i.Versions).ThenInclude(iv => iv.Values).ThenInclude(v => v.Field)
            .AsNoTracking()
            .Where(i => i.Item.ItemLayout.Id == request.ItemLayoutId)
            .ToListAsync(cancellationToken);

        return assets;
    }
}