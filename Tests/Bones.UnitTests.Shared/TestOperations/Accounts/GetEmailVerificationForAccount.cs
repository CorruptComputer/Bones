using Bones.Database;
using Bones.Database.DbSets;
using MediatR;

namespace Bones.UnitTests.Shared.TestOperations.Accounts;

/// <summary>
///     TEST - Query for getting the accounts Email Verifications.
/// </summary>
/// <param name="AccountId">ID of the account to check.</param>
public record GetEmailVerificationForAccountQuery(Guid AccountId) : IRequest<IEnumerable<AccountEmailVerification>>;

internal class GetEmailVerificationForAccountHandler(BonesDbContext dbContext)
    : IRequestHandler<GetEmailVerificationForAccountQuery, IEnumerable<AccountEmailVerification>>
{
    public Task<IEnumerable<AccountEmailVerification>> Handle(GetEmailVerificationForAccountQuery request,
        CancellationToken cancellationToken)
    {
        IEnumerable<AccountEmailVerification> verifications = dbContext.AccountEmailVerifications
            .OrderBy(v => v.AccountId)
            .Where(v => v.AccountId == request.AccountId);

        return Task.FromResult(verifications);
    }
}