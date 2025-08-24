using Bones.Database.DbSets.Audit;

namespace Bones.Database.Operations.Audit;

/// <inheritdoc />
public class GetAccountAuditsByAccountIdDb(BonesDbContext dbContext) : IRequestHandler<GetAccountAuditsByAccountIdDb.Query, QueryResponse<List<AccountAudit>>>
{
    /// <summary>
    ///   DB Query to get all audits for a given account
    /// </summary>
    /// <param name="BonesUsedId">The account which was acted upon</param>
    public record Query(Guid BonesUsedId) : IRequest<QueryResponse<List<AccountAudit>>>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Query>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.BonesUsedId).NotNull().NotEqual(Guid.Empty);
        }
    }

    /// <inheritdoc />
    public async Task<QueryResponse<List<AccountAudit>>> Handle(Query request, CancellationToken cancellationToken)
    {
        return await dbContext.AccountAudits
            .Include(x => x.Account)
            .Include(x => x.ActionTakenBy)
            .Where(x => x.Account.Id == request.BonesUsedId)
            .ToListAsync(cancellationToken);
    }
}
