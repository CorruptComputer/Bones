using Bones.Database.DbSets.AccountManagement;

namespace Bones.Database.Operations.AccountManagement;

/// <inheritdoc />
public class InvalidateAllSessionsForUserDb(BonesDbContext dbContext) : IRequestHandler<InvalidateAllSessionsForUserDb.Command, CommandResponse>
{
    /// <summary>
    ///   Invalidates the users sessions
    /// </summary>
    /// <param name="UserId"></param>
    /// <param name="ExcludeSessions"></param>
    public sealed record Command(Guid UserId, List<Guid>? ExcludeSessions = null) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.UserId).NotNull().NotEqual(Guid.Empty);
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        IQueryable<BonesUserSession> userSessions = dbContext.UserSessions
            .Where(x => x.User.Id == request.UserId);

        if (request.ExcludeSessions != null && request.ExcludeSessions.Count != 0)
        {
            userSessions = userSessions.Where(x => !request.ExcludeSessions.Contains(x.Id));
        }

        foreach (BonesUserSession session in userSessions)
        {
            session.IsInvalidated = true;
            dbContext.UserSessions.Update(session);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass();
    }
}
