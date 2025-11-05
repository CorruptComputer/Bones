using Bones.Database.DbSets.Items;

namespace Bones.Database.Operations.Item;

/// <inheritdoc />
public sealed class GetItemLayoutsByProjectDb(BonesDbContext dbContext) : IRequestHandler<GetItemLayoutsByProjectDb.Query, QueryResponse<List<ItemLayout>>>
{
    /// <summary>
    ///   DB Query for getting the item layouts in a project
    /// </summary>
    /// <param name="ProjectId">Internal ID of the project</param>
    public record Query(Guid ProjectId) : IRequest<QueryResponse<List<ItemLayout>>>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Query>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.ProjectId).NotNull().NotEqual(Guid.Empty);
        }
    }

    /// <inheritdoc />
    public async Task<QueryResponse<List<ItemLayout>>> Handle(Query request, CancellationToken cancellationToken)
    {
        return await dbContext.ItemLayouts
            .Include(x => x.Versions)
            .ThenInclude(x => x.FieldLinks)
            .ThenInclude(x => x.FieldVersion)
            .Where(x => x.Project.Id == request.ProjectId)
            .ToListAsync(cancellationToken);
    }
}