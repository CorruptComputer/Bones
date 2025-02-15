using Bones.Database.DbSets.GenericItems;

namespace Bones.Database.Operations.GenericItem;

/// <inheritdoc />
public sealed class GetItemLayoutByProjectAndFriendlyIdPrefixDb(BonesDbContext dbContext)
    : IRequestHandler<GetItemLayoutByProjectAndFriendlyIdPrefixDb.Query, QueryResponse<GenericItemLayout?>>
{
    /// <summary>
    ///     DB Query for getting an item layout by its ID
    /// </summary>
    /// <param name="ProjectId">Internal ID of the item layout</param>
    /// <param name="FriendlyIdPrefix"></param>
    public record Query(Guid ProjectId, string FriendlyIdPrefix) : IRequest<QueryResponse<GenericItemLayout?>>;


    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.ProjectId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.FriendlyIdPrefix).NotNull().NotEmpty().MaximumLength(6).Matches(@"^[a-zA-Z]*$");
        }
    }

    /// <inheritdoc />
    public async Task<QueryResponse<GenericItemLayout?>> Handle(Query request, CancellationToken cancellationToken)
    {
        return await dbContext.ItemLayouts
            .Include(x => x.Versions)
            .ThenInclude(x => x.Fields)
            .FirstOrDefaultAsync(x => x.ProjectId == request.ProjectId && x.FriendlyIdPrefix == request.FriendlyIdPrefix, cancellationToken);
    }
}
