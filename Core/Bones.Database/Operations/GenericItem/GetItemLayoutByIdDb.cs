using Bones.Database.DbSets.GenericItems;

namespace Bones.Database.Operations.GenericItem;

/// <inheritdoc />
public sealed class GetItemLayoutByIdDb(BonesDbContext dbContext) : IRequestHandler<GetItemLayoutByIdDb.Query, QueryResponse<GenericItemLayout?>>
{
    /// <summary>
    ///     DB Query for getting an item layout by its ID
    /// </summary>
    /// <param name="ItemLayoutId">Internal ID of the item layout</param>
    public record Query(Guid ItemLayoutId) : IRequest<QueryResponse<GenericItemLayout?>>;

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
    public async Task<QueryResponse<GenericItemLayout?>> Handle(Query request, CancellationToken cancellationToken)
    {
        return await dbContext.ItemLayouts
            .Include(x => x.Versions)
            .ThenInclude(x => x.Fields)
            .FirstOrDefaultAsync(x => x.Id == request.ItemLayoutId, cancellationToken);
    }
}