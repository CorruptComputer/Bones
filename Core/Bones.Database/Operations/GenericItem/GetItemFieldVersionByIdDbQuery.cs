using Bones.Database.DbSets.GenericItems;

namespace Bones.Database.Operations.GenericItem;

/// <summary>
///     DB Query for getting an item field by its ID
/// </summary>
/// <param name="ItemFieldVersionId">Internal ID of the item field</param>
public record GetItemFieldVersionByIdDbQuery(Guid ItemFieldVersionId) : IRequest<QueryResponse<GenericItemFieldVersion?>>;

internal sealed class GetItemFieldVersionByIdDbQueryValidator : AbstractValidator<GetItemFieldVersionByIdDbQuery>
{

}

internal sealed class GetItemFieldVersionByIdDbHandler(BonesDbContext dbContext) : IRequestHandler<GetItemFieldVersionByIdDbQuery, QueryResponse<GenericItemFieldVersion?>>
{
    public async Task<QueryResponse<GenericItemFieldVersion?>> Handle(GetItemFieldVersionByIdDbQuery request, CancellationToken cancellationToken)
    {
        return await dbContext.ItemFieldVersions
            .Include(x => x.PossibleValues)
            .FirstOrDefaultAsync(x => x.Id == request.ItemFieldVersionId, cancellationToken);
    }
}