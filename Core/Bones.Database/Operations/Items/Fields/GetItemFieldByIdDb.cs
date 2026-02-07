using Bones.Database.DbSets.Items.Fields;

namespace Bones.Database.Operations.Items.Fields;


/// <inheritdoc />
public sealed class GetItemFieldByIdDb(BonesDbContext dbContext) : IRequestHandler<GetItemFieldByIdDb.Query, QueryResponse<ItemField?>>
{
    /// <summary>
    ///   DB Query for getting an item field by its ID
    /// </summary>
    /// <param name="ItemFieldId">Internal ID of the item field</param>
    public record Query(Guid ItemFieldId) : IRequest<QueryResponse<ItemField?>>;

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
    public async Task<QueryResponse<ItemField?>> Handle(Query request, CancellationToken cancellationToken)
    {
        return await dbContext.ItemFields
            .Include(x => x.Versions)
            .ThenInclude(x => x.PossibleValues)
            .Include(x => x.Project)
            .FirstOrDefaultAsync(x => x.Id == request.ItemFieldId, cancellationToken);
    }
}