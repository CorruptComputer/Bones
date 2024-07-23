using Bones.Database.DbSets;
using Bones.Database.DbSets.Identity;
using Microsoft.EntityFrameworkCore;

namespace Bones.Database.Operations.Users;

public class VerifyUserEmailDb(BonesDbContext dbContext) : IRequestHandler<VerifyUserEmailDb.Command, CommandResponse>
{
    /// <summary>
    ///     DB Command for verifying an email address for an User.
    /// </summary>
    /// <param name="UserId">ID of the User</param>
    /// <param name="Token">Verification can</param>
    public record Command(Guid UserId, Guid Token) : IRequest<CommandResponse>;
    
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        IQueryable<UserEmailVerification> validMatches = dbContext.UserEmailVerifications.Where(verification =>
            verification.UserId == request.UserId
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

        BonesUser? acct = await dbContext.Users.FirstOrDefaultAsync(User => User.Id == request.UserId,
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
        acct.EmailConfirmedDateTime = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);

        return new()
        {
            Success = true
        };
    }
}