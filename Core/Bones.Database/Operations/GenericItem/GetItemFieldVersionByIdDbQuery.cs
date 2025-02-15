using Bones.Database.DbSets.GenericItems;

namespace Bones.Database.Operations.GenericItem;

/// <inheritdoc />
public sealed class GetItemFieldVersionByIdDb(BonesDbContext dbContext) : IRequestHandler<GetItemFieldVersionByIdDb.Query, QueryResponse<GenericItemFieldVersion?>>
{
    /// <summary>
    ///     DB Query for getting an item field by its ID
    /// </summary>
    /// <param name="ItemFieldVersionId">Internal ID of the item field</param>
    public record Query(Guid ItemFieldVersionId) : IRequest<QueryResponse<GenericItemFieldVersion?>>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Query>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.ItemFieldVersionId).NotNull().NotEqual(Guid.Empty);
        }
    }

    /// <inheritdoc />
    public async Task<QueryResponse<GenericItemFieldVersion?>> Handle(Query request, CancellationToken cancellationToken)
    {
        return await dbContext.ItemFieldVersions
            .Include(x => x.PossibleValues)
            .FirstOrDefaultAsync(x => x.Id == request.ItemFieldVersionId, cancellationToken);
    }
}