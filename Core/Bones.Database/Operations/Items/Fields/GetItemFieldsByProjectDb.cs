using Bones.Database.DbSets.Items.Fields;

namespace Bones.Database.Operations.Items.Fields;

/// <inheritdoc />
public sealed class GetItemFieldsByProjectDb(BonesDbContext dbContext) : IRequestHandler<GetItemFieldsByProjectDb.Query, QueryResponse<List<ItemField>>>
{
    /// <summary>
    ///   DB Query for getting the item fields in a project
    /// </summary>
    /// <param name="ProjectId">Internal ID of the project</param>
    public record Query(Guid ProjectId) : IRequest<QueryResponse<List<ItemField>>>;

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
    public async Task<QueryResponse<List<ItemField>>> Handle(Query request, CancellationToken cancellationToken)
    {
        return await dbContext.ItemFields
            .Include(x => x.Versions)
            .ThenInclude(x => x.PossibleValues)
            .Where(x => x.ProjectId == request.ProjectId)
            .ToListAsync(cancellationToken);
    }
}