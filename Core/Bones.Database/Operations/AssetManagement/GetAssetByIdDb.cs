using Bones.Database.DbSets.AssetManagement;

namespace Bones.Database.Operations.AssetManagement;

/// <inheritdoc />
public sealed class GetAssetByIdDb(BonesDbContext dbContext) : IRequestHandler<GetAssetByIdDb.Query, QueryResponse<Asset?>>
{
    /// <summary>
    ///   DB Command for getting an Asset by id.
    /// </summary>
    /// <param name="AssetId">ID of the item</param>
    public sealed record Query(Guid AssetId) : IRequest<QueryResponse<Asset?>>;

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
        Asset? asset = await dbContext.Assets
            .Include(a => a.Project)
            .Include(a => a.Item).ThenInclude(i => i.ItemLayout)
            .Include(a => a.Item).ThenInclude(i => i.Versions).ThenInclude(iv => iv.ItemLayoutVersion).ThenInclude(lv => lv.FieldLinks).ThenInclude(fl => fl.FieldVersion).ThenInclude(fv => fv.PossibleValues)
            .Include(a => a.Item).ThenInclude(i => i.Versions).ThenInclude(iv => iv.Values).ThenInclude(v => v.Field)
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == request.AssetId, cancellationToken);

        return asset;
    }
}