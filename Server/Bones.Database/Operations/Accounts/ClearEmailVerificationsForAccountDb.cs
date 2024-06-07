using Bones.Database.DbSets;
using Bones.Database.Models;
using MediatR;

namespace Bones.Database.Operations.Accounts;

/// <summary>
/// </summary>
/// <param name="AccountId"></param>
public record ClearEmailVerificationsForAccountDbCommand(long AccountId) : IRequest<DbCommandResponse>;

internal class ClearEmailVerificationsForAccountDbHandler(BonesDbContext dbContext)
    : IRequestHandler<ClearEmailVerificationsForAccountDbCommand, DbCommandResponse>
{
    public async Task<DbCommandResponse> Handle(ClearEmailVerificationsForAccountDbCommand request,
        CancellationToken cancellationToken)
    {
        IEnumerable<AccountEmailVerification> oldVerifications = dbContext.AccountEmailVerifications
            .OrderBy(v => v.AccountId)
            .Where(v => v.AccountId == request.AccountId)
            .AsEnumerable();

        dbContext.AccountEmailVerifications.RemoveRange(oldVerifications);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new()
        {
            Success = true
        };
    }
}