using Bones.Database.DbSets.GenericItems.GenericItemFields;

namespace Bones.Database.Operations.GenericItem;

/// <summary>
///     DB Query for getting the item fields in a project
/// </summary>
/// <param name="ProjectId">Internal ID of the project</param>
public record GetItemFieldsByProjectDbQuery(Guid ProjectId) : IRequest<QueryResponse<List<GenericItemField>>>;

internal sealed class GetItemFieldsByProjectDbQueryValidator : AbstractValidator<GetItemFieldsByProjectDbQuery>
{

}

internal sealed class GetItemFieldsByProjectDbHandler(BonesDbContext dbContext) : IRequestHandler<GetItemFieldsByProjectDbQuery, QueryResponse<List<GenericItemField>>>
{
    public async Task<QueryResponse<List<GenericItemField>>> Handle(GetItemFieldsByProjectDbQuery request, CancellationToken cancellationToken)
    {
        return await dbContext.ItemFields
            .Include(x => x.Versions)
            .ThenInclude(x => x.PossibleValues)
            .Where(x => x.ProjectId == request.ProjectId)
            .ToListAsync(cancellationToken);
    }
}