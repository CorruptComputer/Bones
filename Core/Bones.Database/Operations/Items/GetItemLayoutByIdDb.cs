using Bones.Database.DbSets.Items;

namespace Bones.Database.Operations.Items;

/// <inheritdoc />
public sealed class GetItemLayoutByIdDb(BonesDbContext dbContext) : IRequestHandler<GetItemLayoutByIdDb.Query, QueryResponse<ItemLayout?>>
{
    /// <summary>
    ///   DB Query for getting an item layout by its ID
    /// </summary>
    /// <param name="ItemLayoutId">Internal ID of the item layout</param>
    public record Query(Guid ItemLayoutId) : IRequest<QueryResponse<ItemLayout?>>;

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
    public async Task<QueryResponse<ItemLayout?>> Handle(Query request, CancellationToken cancellationToken)
    {
        return await dbContext.ItemLayouts
            .Include(x => x.Project)
            .Include(x => x.Versions)
            .ThenInclude(x => x.FieldLinks)
            .ThenInclude(x => x.FieldVersion)
            .ThenInclude(x => x.ItemField)
            .Include(x => x.Versions)
            .ThenInclude(x => x.FieldLinks)
            .ThenInclude(x => x.FieldVersion)
            .ThenInclude(x => x.PossibleValues)
            .Include(x => x.Versions)
            .ThenInclude(x => x.AssigneeSlots)
            .FirstOrDefaultAsync(x => x.Id == request.ItemLayoutId, cancellationToken);
    }
}