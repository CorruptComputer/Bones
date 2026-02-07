using Bones.Database.DbSets.Accounts;

namespace Bones.Database.Operations.Accounts;

/// <inheritdoc />
public class InvalidateBonesUserSessionDb(BonesDbContext dbContext) : IRequestHandler<InvalidateBonesUserSessionDb.Command, CommandResponse>
{
    /// <summary>
    ///   Invalidates the user session
    /// </summary>
    /// <param name="SessionId"></param>
    public sealed record Command(Guid SessionId) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.SessionId).NotNull().NotEqual(Guid.Empty);
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        BonesUserSession? session = await dbContext.UserSessions
            .FirstOrDefaultAsync(x => x.Id == request.SessionId, cancellationToken);

        if (session is null)
        {
            return CommandResponse.Fail("Session not found");
        }

        session.IsInvalidated = true;
        dbContext.UserSessions.Update(session);

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass();
    }
}
