using Bones.Database.DbSets.GenericItems;

namespace Bones.Database.Operations.GenericItem;

/// <summary>
///     DB Query for getting the item layouts in a project
/// </summary>
/// <param name="ProjectId">Internal ID of the project</param>
public record GetItemLayoutsByProjectDbQuery(Guid ProjectId) : IRequest<QueryResponse<List<GenericItemLayout>>>;

internal sealed class GetItemLayoutsByProjectDbQueryValidator : AbstractValidator<GetItemLayoutsByProjectDbQuery>
{

}

internal sealed class GetItemLayoutsByProjectDbHandler(BonesDbContext dbContext) : IRequestHandler<GetItemLayoutsByProjectDbQuery, QueryResponse<List<GenericItemLayout>>>
{
    public async Task<QueryResponse<List<GenericItemLayout>>> Handle(GetItemLayoutsByProjectDbQuery request, CancellationToken cancellationToken)
    {
        return await dbContext.ItemLayouts
            .Include(x => x.Versions)
            .ThenInclude(x => x.Fields)
            .Where(x => x.ProjectId == request.ProjectId)
            .ToListAsync(cancellationToken);
    }
}