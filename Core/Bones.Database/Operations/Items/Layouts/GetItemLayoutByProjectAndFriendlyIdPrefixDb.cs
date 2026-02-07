using Bones.Database.DbSets.Items.Layouts;

namespace Bones.Database.Operations.Items.Layouts;

/// <inheritdoc />
public sealed class GetItemLayoutByProjectAndFriendlyIdPrefixDb(BonesDbContext dbContext)
    : IRequestHandler<GetItemLayoutByProjectAndFriendlyIdPrefixDb.Query, QueryResponse<ItemLayout?>>
{
    /// <summary>
    ///   DB Query for getting an item layout by its ID
    /// </summary>
    /// <param name="ProjectId">Internal ID of the item layout</param>
    /// <param name="FriendlyIdPrefix"></param>
    public record Query(Guid ProjectId, string FriendlyIdPrefix) : IRequest<QueryResponse<ItemLayout?>>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Query>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.ProjectId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.FriendlyIdPrefix).NotEmpty().MaximumLength(6).Matches(@"^[a-zA-Z]*$");
        }
    }

    /// <inheritdoc />
    public async Task<QueryResponse<ItemLayout?>> Handle(Query request, CancellationToken cancellationToken)
    {
        return await dbContext.ItemLayouts
            .Include(x => x.Versions)
            .ThenInclude(x => x.ItemLayoutFieldVersionLinks)
            .ThenInclude(x => x.ItemFieldVersion)
            .FirstOrDefaultAsync(x => x.ProjectId == request.ProjectId && x.FriendlyIdPrefix == request.FriendlyIdPrefix, cancellationToken);
    }
}
