using Bones.Database.DbSets.Items.Fields;

namespace Bones.Database.Operations.Items.Fields;

/// <inheritdoc />
public sealed class GetItemFieldVersionByIdDb(BonesDbContext dbContext) : IRequestHandler<GetItemFieldVersionByIdDb.Query, QueryResponse<ItemFieldVersion?>>
{
    /// <summary>
    ///   DB Query for getting an item field by its ID
    /// </summary>
    /// <param name="ItemFieldVersionId">Internal ID of the item field</param>
    public record Query(Guid ItemFieldVersionId) : IRequest<QueryResponse<ItemFieldVersion?>>;

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
    public async Task<QueryResponse<ItemFieldVersion?>> Handle(Query request, CancellationToken cancellationToken)
    {
        return await dbContext.ItemFieldVersions
            .Include(x => x.PossibleValues)
            .Include(x => x.ItemField)
            .FirstOrDefaultAsync(x => x.Id == request.ItemFieldVersionId, cancellationToken);
    }
}