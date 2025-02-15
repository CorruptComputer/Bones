using Bones.Database.DbSets.GenericItems;

namespace Bones.Database.Operations.GenericItem;

/// <summary>
///     DB Query for getting an item field by its ID
/// </summary>
/// <param name="ItemFieldId">Internal ID of the item field</param>
public record GetItemFieldByIdDbQuery(Guid ItemFieldId) : IRequest<QueryResponse<GenericItemField?>>;

internal sealed class GetItemFieldByIdDbQueryValidator : AbstractValidator<GetItemFieldByIdDbQuery>
{

}

internal sealed class GetItemFieldByIdDbHandler(BonesDbContext dbContext) : IRequestHandler<GetItemFieldByIdDbQuery, QueryResponse<GenericItemField?>>
{
    public async Task<QueryResponse<GenericItemField?>> Handle(GetItemFieldByIdDbQuery request, CancellationToken cancellationToken)
    {
        return await dbContext.ItemFields
            .Include(x => x.Versions)
            .ThenInclude(x => x.PossibleValues)
            .FirstOrDefaultAsync(x => x.Id == request.ItemFieldId, cancellationToken);
    }
}