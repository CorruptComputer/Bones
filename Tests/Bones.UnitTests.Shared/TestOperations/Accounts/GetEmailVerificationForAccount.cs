using Bones.Database;
using Bones.Database.DbSets;
using MediatR;

namespace Bones.UnitTests.Shared.TestOperations.Accounts;

/// <summary>
///   TEST - Query for getting the accounts Email Verifications.
/// </summary>
/// <param name="AccountId">ID of the account to check.</param>
public record GetEmailVerificationForAccountQuery(long AccountId) : IRequest<IEnumerable<GetEmailVerificationForAccountResponse>>;

/// <summary>
///   TEST - Response of the accounts Email Verifications.
/// </summary>
/// <param name="EmailVerificationId">The ID of the verification.</param>
/// <param name="AccountId">The ID of the account the verification is for.</param>
/// <param name="Token">Verification token.</param>
public record GetEmailVerificationForAccountResponse(long EmailVerificationId, long AccountId, Guid Token);

internal class GetEmailVerificationForAccountHandler(BonesDbContext dbContext) : IRequestHandler<GetEmailVerificationForAccountQuery, IEnumerable<GetEmailVerificationForAccountResponse>>
{
    public Task<IEnumerable<GetEmailVerificationForAccountResponse>> Handle(GetEmailVerificationForAccountQuery request, CancellationToken cancellationToken)
    {
        IQueryable<AccountEmailVerification> verifications = dbContext.AccountEmailVerifications
            .OrderBy(v => v.AccountId)
            .Where(v => v.AccountId == request.AccountId);

        IEnumerable<GetEmailVerificationForAccountResponse> response = verifications.Select(v => new GetEmailVerificationForAccountResponse(v.EmailVerificationId, v.AccountId, v.Token));

        return Task.FromResult(response);
    }
}