using Bones.Database.DbSets;
using Microsoft.EntityFrameworkCore;

namespace Bones.Database.Operations.Users;

public class ClearExpiredEmailVerificationsDb(BonesDbContext dbContext) : IRequestHandler<ClearExpiredEmailVerificationsDb.Query, QueryResponse<int>>
{
    /// <summary>
    ///     DB Query for clearing the expired email verifications, returns the number removed.
    /// </summary>
    public record Query : IRequest<QueryResponse<int>>;
    
    public async Task<QueryResponse<int>> Handle(Query request, CancellationToken cancellationToken)
    {
        IQueryable<UserEmailVerification> expired =
            dbContext.UserEmailVerifications.Where(v => v.ValidUntilDateTime < DateTimeOffset.UtcNow);
        int count = await expired.CountAsync(cancellationToken);

        if (count > 0)
        {
            await expired.ExecuteDeleteAsync(cancellationToken);
        }

        return new()
        {
            Success = true,
            Result = count
        };
    }
}