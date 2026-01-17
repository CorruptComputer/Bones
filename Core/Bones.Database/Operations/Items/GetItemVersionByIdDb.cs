using Bones.Database.DbSets.Items;

namespace Bones.Database.Operations.Items;

/// <inheritdoc />
public sealed class GetItemVersionByIdDb(BonesDbContext dbContext) : IRequestHandler<GetItemVersionByIdDb.Query, QueryResponse<ItemVersion?>>
{
    /// <summary>
    ///   DB Query for getting an item version by its ID
    /// </summary>
    /// <param name="ItemVersionId">Internal ID of the item version</param>
    /// <param name="IncludeLayoutVersion">Whether to include the related layout version</param>
    /// <param name="IncludeValues">Whether to include the item values</param>
    /// <param name="IncludeAssignees">Whether to include the assignees</param>
    public record Query(Guid ItemVersionId, bool IncludeLayoutVersion = false, bool IncludeValues = false, bool IncludeAssignees = false) : IRequest<QueryResponse<ItemVersion?>>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Query>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.ItemVersionId).NotNull().NotEqual(Guid.Empty);
        }
    }

    /// <inheritdoc />
    public async Task<QueryResponse<ItemVersion?>> Handle(Query request, CancellationToken cancellationToken)
    {
        IQueryable<ItemVersion> query = dbContext.ItemVersions.AsNoTracking().Include(iv => iv.Item);

        if (request.IncludeLayoutVersion)
        {
            query = query.Include(iver => iver.ItemLayoutVersion)
                         .ThenInclude(ilv => ilv!.ItemLayoutFieldVersionLinks);
        }

        if (request.IncludeValues)
        {
            query = query.Include(x => x.ItemValues)
                         .ThenInclude(ival => ival.ItemFieldVersion);
        }

        if (request.IncludeAssignees)
        {
            query = query.Include(x => x.ItemAssignees);
        }

        return await query.FirstOrDefaultAsync(x => x.Id == request.ItemVersionId, cancellationToken);
    }
}