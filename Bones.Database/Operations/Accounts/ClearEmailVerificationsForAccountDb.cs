using Bones.Database.DbSets;

namespace Bones.Database.Operations.Accounts;

/// <summary>
/// </summary>
/// <param name="AccountId"></param>
public record ClearEmailVerificationsForAccountDbCommand(Guid AccountId) : IRequest<CommandResponse>;

internal class ClearEmailVerificationsForAccountDbHandler(BonesDbContext dbContext)
    : IRequestHandler<ClearEmailVerificationsForAccountDbCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(ClearEmailVerificationsForAccountDbCommand request,
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