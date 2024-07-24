using Bones.Database;
using Bones.Database.DbSets.Identity;
using MediatR;

namespace Bones.Testing.Shared.TestOperations.Users;

public class GetEmailVerificationForUser(BonesDbContext dbContext) : IRequestHandler<GetEmailVerificationForUser.Query, IEnumerable<UserEmailVerification>>
{
    /// <summary>
    ///     TEST - Query for getting the accounts Email Verifications.
    /// </summary>
    /// <param name="AccountId">ID of the account to check.</param>
    public record Query(Guid AccountId) : IRequest<IEnumerable<UserEmailVerification>>;
    
    public Task<IEnumerable<UserEmailVerification>> Handle(Query request,
        CancellationToken cancellationToken)
    {
        IEnumerable<UserEmailVerification> verifications = dbContext.UserEmailVerifications
            .Where(v => v.User.Id == request.AccountId);

        return Task.FromResult(verifications);
    }
}