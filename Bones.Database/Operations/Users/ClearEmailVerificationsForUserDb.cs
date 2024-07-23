using Bones.Database.DbSets;

namespace Bones.Database.Operations.Users;

public class ClearEmailVerificationsForUserDb(BonesDbContext dbContext) : IRequestHandler<ClearEmailVerificationsForUserDb.Command, CommandResponse>
{
    /// <summary>
    /// </summary>
    /// <param name="UserId"></param>
    public record Command(Guid UserId) : IRequest<CommandResponse>;
    
    public async Task<CommandResponse> Handle(Command request,
        CancellationToken cancellationToken)
    {
        IEnumerable<UserEmailVerification> oldVerifications = dbContext.UserEmailVerifications
            .OrderBy(v => v.UserId)
            .Where(v => v.UserId == request.UserId)
            .AsEnumerable();

        dbContext.UserEmailVerifications.RemoveRange(oldVerifications);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new()
        {
            Success = true
        };
    }
}