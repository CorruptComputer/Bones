using Bones.Database.DbSets;
using Bones.Database.DbSets.Identity;
using Microsoft.EntityFrameworkCore;

namespace Bones.Database.Operations.Accounts;

/// <summary>
///     DB Command for verifying an email address for an account.
/// </summary>
/// <param name="AccountId">ID of the account</param>
/// <param name="Token">Verification can</param>
public record VerifyAccountEmailDbCommand(Guid AccountId, Guid Token) : IRequest<CommandResponse>;

internal class VerifyAccountEmailDbHandler(BonesDbContext dbContext)
    : IRequestHandler<VerifyAccountEmailDbCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(VerifyAccountEmailDbCommand request,
        CancellationToken cancellationToken)
    {
        IQueryable<AccountEmailVerification> validMatches = dbContext.AccountEmailVerifications.Where(verification =>
            verification.AccountId == request.AccountId
            && verification.Token == request.Token
            && verification.ValidUntilDateTime > DateTime.UtcNow);

        if (!validMatches.Any())
        {
            return new()
            {
                Success = false,
                FailureReason = "Supplied information is invalid or the verification time has expired."
            };
        }

        await validMatches.ExecuteDeleteAsync(cancellationToken);

        Account? acct = await dbContext.Accounts.FirstOrDefaultAsync(account => account.Id == request.AccountId,
            cancellationToken);

        if (acct == null)
        {
            return new()
            {
                Success = false,
                FailureReason = "Failed to find the specified account."
            };
        }

        acct.EmailConfirmed = true;
        acct.EmailConfirmedDateTime = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);

        return new()
        {
            Success = true
        };
    }
}