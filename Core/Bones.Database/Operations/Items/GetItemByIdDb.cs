using Bones.Database.DbSets.Items;

namespace Bones.Database.Operations.Items;

/// <inheritdoc />
public sealed class GetItemByIdDb(BonesDbContext dbContext) : IRequestHandler<GetItemByIdDb.Query, QueryResponse<Item?>>
{
    /// <summary>
    ///   DB Query for getting an item by its ID
    /// </summary>
    /// <param name="ItemId">Internal ID of the item</param>
    /// <param name="IncludeProject">Whether to include the related project</param>
    /// <param name="IncludeVersions">Whether to include the item versions</param>
    public record Query(Guid ItemId, bool IncludeProject = false, bool IncludeVersions = false) : IRequest<QueryResponse<Item?>>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Query>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.ItemId).NotNull().NotEqual(Guid.Empty);
        }
    }

    /// <inheritdoc />
    public async Task<QueryResponse<Item?>> Handle(Query request, CancellationToken cancellationToken)
    {
        IQueryable<Item> query = dbContext.Items.AsNoTracking();

        if (request.IncludeProject)
        {
            query = query.Include(x => x.Project);
        }

        if (request.IncludeVersions)
        {
            query = query.Include(x => x.Versions);
        }

        return await query.FirstOrDefaultAsync(x => x.Id == request.ItemId, cancellationToken);
    }
}