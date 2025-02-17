using Bones.Database.DbSets.GenericItems;

namespace Bones.Database.Operations.GenericItem;


/// <inheritdoc />
public sealed class GetItemFieldByIdDb(BonesDbContext dbContext) : IRequestHandler<GetItemFieldByIdDb.Query, QueryResponse<GenericItemField?>>
{
    /// <summary>
    ///     DB Query for getting an item field by its ID
    /// </summary>
    /// <param name="ItemFieldId">Internal ID of the item field</param>
    public record Query(Guid ItemFieldId) : IRequest<QueryResponse<GenericItemField?>>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Query>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.ItemFieldId).NotNull().NotEqual(Guid.Empty);
        }
    }

    /// <inheritdoc />
    public async Task<QueryResponse<GenericItemField?>> Handle(Query request, CancellationToken cancellationToken)
    {
        return await dbContext.ItemFields
            .Include(x => x.Versions)
            .ThenInclude(x => x.PossibleValues)
            .Include(x => x.Project)
            .FirstOrDefaultAsync(x => x.Id == request.ItemFieldId, cancellationToken);
    }
}