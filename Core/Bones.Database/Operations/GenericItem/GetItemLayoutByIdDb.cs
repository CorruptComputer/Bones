using Bones.Database.DbSets.GenericItems;

namespace Bones.Database.Operations.GenericItem;

/// <summary>
///     DB Query for getting an item layout by its ID
/// </summary>
/// <param name="ItemLayoutId">Internal ID of the item layout</param>
public record GetItemLayoutByIdDbQuery(Guid ItemLayoutId) : IRequest<QueryResponse<GenericItemLayout?>>;

internal sealed class GetItemLayoutByIdDbQueryValidator : AbstractValidator<GetItemLayoutByIdDbQuery>
{

}

internal sealed class GetItemLayoutByIdDbHandler(BonesDbContext dbContext) : IRequestHandler<GetItemLayoutByIdDbQuery, QueryResponse<GenericItemLayout?>>
{
    public async Task<QueryResponse<GenericItemLayout?>> Handle(GetItemLayoutByIdDbQuery request, CancellationToken cancellationToken)
    {
        return await dbContext.ItemLayouts
            .Include(x => x.Versions)
            .ThenInclude(x => x.Fields)
            .FirstOrDefaultAsync(x => x.Id == request.ItemLayoutId, cancellationToken);
    }
}