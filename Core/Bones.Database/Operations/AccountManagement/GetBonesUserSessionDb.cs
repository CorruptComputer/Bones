using Bones.Database.DbSets.AccountManagement;

namespace Bones.Database.Operations.AccountManagement;

/// <inheritdoc />
public class GetBonesUserSessionDb(BonesDbContext dbContext) : IRequestHandler<GetBonesUserSessionDb.Query, QueryResponse<BonesUserSession?>>
{
    /// <summary>
    ///   DB Query for getting a user session by ID
    /// </summary>
    /// <param name="SessionId"></param>
    public sealed record Query(Guid SessionId) : IRequest<QueryResponse<BonesUserSession?>>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Query>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.SessionId).NotNull().NotEqual(Guid.Empty);
        }
    }

    /// <inheritdoc />
    public async Task<QueryResponse<BonesUserSession?>> Handle(Query request, CancellationToken cancellationToken)
    {
        BonesUserSession? session = await dbContext.UserSessions
            .FirstOrDefaultAsync(x => x.Id == request.SessionId, cancellationToken);

        if (session is not null)
        {
            session.LastAccessedDateTime = DateTimeOffset.UtcNow;
            dbContext.UserSessions.Update(session);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return session;
    }
}
