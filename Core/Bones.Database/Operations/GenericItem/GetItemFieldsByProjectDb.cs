using Bones.Database.DbSets.GenericItems;

namespace Bones.Database.Operations.GenericItem;

/// <inheritdoc />
public sealed class GetItemFieldsByProjectDb(BonesDbContext dbContext) : IRequestHandler<GetItemFieldsByProjectDb.Query, QueryResponse<List<GenericItemField>>>
{
    /// <summary>
    ///     DB Query for getting the item fields in a project
    /// </summary>
    /// <param name="ProjectId">Internal ID of the project</param>
    public record Query(Guid ProjectId) : IRequest<QueryResponse<List<GenericItemField>>>;

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
    public async Task<QueryResponse<List<GenericItemField>>> Handle(Query request, CancellationToken cancellationToken)
    {
        return await dbContext.ItemFields
            .Include(x => x.Versions)
            .ThenInclude(x => x.PossibleValues)
            .Where(x => x.Project.Id == request.ProjectId)
            .ToListAsync(cancellationToken);
    }
}