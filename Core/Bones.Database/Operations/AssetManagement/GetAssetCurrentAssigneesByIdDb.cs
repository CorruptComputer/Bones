using Bones.Database.DbSets.AssetManagement;

namespace Bones.Database.Operations.AssetManagement;

/// <inheritdoc />
public sealed class GetAssetCurrentAssigneesByIdDb(BonesDbContext dbContext) : IRequestHandler<GetAssetCurrentAssigneesByIdDb.Query, QueryResponse<Asset?>>
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
            .Include(a => a.Item).ThenInclude(i => i.Versions).ThenInclude(iv => iv.Assignees).ThenInclude(a => a.Slot)
            .Include(a => a.Item).ThenInclude(i => i.Versions).ThenInclude(iv => iv.Assignees).ThenInclude(a => a.AssignedUser)
            //.Include(a => a.Item).ThenInclude(i => i.Versions).ThenInclude(iv => iv.Assignees).ThenInclude(a => a.AssignedRole)
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == request.AssetId, cancellationToken);

        return asset;
    }
}