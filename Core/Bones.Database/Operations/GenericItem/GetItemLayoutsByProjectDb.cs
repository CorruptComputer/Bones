using Bones.Database.DbSets.GenericItems;

namespace Bones.Database.Operations.GenericItem;

/// <inheritdoc />
public sealed class GetItemLayoutsByProjectDb(BonesDbContext dbContext) : IRequestHandler<GetItemLayoutsByProjectDb.Query, QueryResponse<List<GenericItemLayout>>>
{
    /// <summary>
    ///   DB Query for getting the item layouts in a project
    /// </summary>
    /// <param name="ProjectId">Internal ID of the project</param>
    public record Query(Guid ProjectId) : IRequest<QueryResponse<List<GenericItemLayout>>>;

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
    public async Task<QueryResponse<List<GenericItemLayout>>> Handle(Query request, CancellationToken cancellationToken)
    {
        return await dbContext.ItemLayouts
            .Include(x => x.Versions)
            .ThenInclude(x => x.FieldLinks)
            .ThenInclude(x => x.FieldVersion)
            .Where(x => x.Project.Id == request.ProjectId)
            .ToListAsync(cancellationToken);
    }
}