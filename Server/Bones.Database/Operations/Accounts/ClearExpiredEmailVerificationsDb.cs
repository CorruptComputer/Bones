using Bones.Database.DbSets;
using Bones.Database.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bones.Database.Operations.Accounts;

/// <summary>
///   DB Query for clearing the expired email verifications, returns the number removed.
/// </summary>
public record ClearExpiredEmailVerificationsDbQuery : IRequest<DbQueryResponse<int>>;

internal class ClearExpiredEmailVerificationsDbHandler(BonesDbContext dbContext) : IRequestHandler<ClearExpiredEmailVerificationsDbQuery, DbQueryResponse<int>>
{
    public async Task<DbQueryResponse<int>> Handle(ClearExpiredEmailVerificationsDbQuery request, CancellationToken cancellationToken)
    {
        IQueryable<AccountEmailVerification> expired = dbContext.AccountEmailVerifications.Where(v => v.ValidUntilDateTime < DateTimeOffset.UtcNow);
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