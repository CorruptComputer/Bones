using Bones.Database.DbSets.Identity;

namespace Bones.Database.Operations.Identity;

public class VerifyUserEmailDb(BonesDbContext dbContext) : IRequestHandler<VerifyUserEmailDb.Command, CommandResponse>
{
    /// <summary>
    ///     DB Command for verifying an email address for a user.
    /// </summary>
    /// <param name="UserId">ID of the User</param>
    /// <param name="Token">Verification can</param>
    public record Command(Guid UserId, Guid Token) : IValidatableRequest<CommandResponse>
    {
        /// <inheritdoc />
        public bool IsRequestValid()
        {
            if (UserId == Guid.Empty)
            {
                return false;
            }

            if (Token == Guid.Empty)
            {
                return false;
            }

            return true;
        }
    }

    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        IQueryable<UserEmailVerification> validMatches = dbContext.UserEmailVerifications.Where(verification =>
            verification.User.Id == request.UserId
            && verification.Token == request.Token
            && verification.ValidUntilDateTime > DateTimeOffset.UtcNow);

        if (!validMatches.Any())
        {
            return new()
            {
                Success = false,
                FailureReason = "Supplied information is invalid or the verification time has expired."
            };
        }

        await validMatches.ExecuteDeleteAsync(cancellationToken);

        BonesUser? acct = await dbContext.Users.FirstOrDefaultAsync(user => user.Id == request.UserId,
            cancellationToken);

        if (acct == null)
        {
            return new()
            {
                Success = false,
                FailureReason = "Failed to find the specified User."
            };
        }

        acct.EmailConfirmed = true;
        acct.EmailConfirmedDateTime = DateTimeOffset.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);

        return new()
        {
            Success = true
        };
    }
}