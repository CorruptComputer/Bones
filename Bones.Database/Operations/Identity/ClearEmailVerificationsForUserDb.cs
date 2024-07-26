using Bones.Database.DbSets.Identity;

namespace Bones.Database.Operations.Identity;

public sealed class ClearEmailVerificationsForUserDb(BonesDbContext dbContext) : IRequestHandler<ClearEmailVerificationsForUserDb.Command, CommandResponse>
{
    /// <summary>
    /// </summary>
    /// <param name="UserId"></param>
    public record Command(Guid UserId) : IValidatableRequest<CommandResponse>
    {
        /// <inheritdoc />
        public bool IsRequestValid()
        {
            if (UserId == Guid.Empty)
            {
                return false;
            }

            return true;
        }
    }

    public async Task<CommandResponse> Handle(Command request,
        CancellationToken cancellationToken)
    {
        IEnumerable<UserEmailVerification> oldVerifications = dbContext.UserEmailVerifications
            .Where(v => v.User.Id == request.UserId)
            .AsEnumerable();

        dbContext.UserEmailVerifications.RemoveRange(oldVerifications);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new()
        {
            Success = true
        };
    }
}