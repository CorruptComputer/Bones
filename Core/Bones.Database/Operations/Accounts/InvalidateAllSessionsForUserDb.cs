using Bones.Database.DbSets.Accounts;

namespace Bones.Database.Operations.Accounts;

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
            RuleFor(x => x.ExcludeSessions).Must(list => list == null || list.All(id => id != Guid.Empty))
                .WithMessage("ExcludeSessions cannot contain empty IDs");
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        IQueryable<BonesUserSession> userSessions = dbContext.UserSessions
            .Where(x => x.UserId == request.UserId);

        if (request.ExcludeSessions != null && request.ExcludeSessions.Count != 0)
        {
            userSessions = userSessions.Where(x => !request.ExcludeSessions.Contains(x.Id));
        }

        List<BonesUserSession> sessionsToInvalidate = await userSessions.ToListAsync(cancellationToken);

        foreach (BonesUserSession session in sessionsToInvalidate)
        {
            session.IsInvalidated = true;
            dbContext.UserSessions.Update(session);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass();
    }
}
