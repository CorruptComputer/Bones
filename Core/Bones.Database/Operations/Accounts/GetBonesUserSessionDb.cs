using System.Net;
using Bones.Database.DbSets.Accounts;
using Bones.Database.DbSets.Audits;

namespace Bones.Database.Operations.Accounts;

/// <inheritdoc />
public class GetBonesUserSessionDb(BonesDbContext dbContext) : IRequestHandler<GetBonesUserSessionDb.Query, QueryResponse<BonesUserSession?>>
{
    /// <summary>
    ///   DB Query for getting a user session by ID
    /// </summary>
    /// <param name="SessionId"></param>
    /// <param name="RequestingIp"></param>
    public sealed record Query(Guid SessionId, IPAddress RequestingIp) : IRequest<QueryResponse<BonesUserSession?>>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Query>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.SessionId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.RequestingIp).NotNull();
        }
    }

    /// <inheritdoc />
    public async Task<QueryResponse<BonesUserSession?>> Handle(Query request, CancellationToken cancellationToken)
    {
        // Check for too many failed attempts from this IP
        DateTimeOffset cutoffTime = DateTimeOffset.UtcNow.AddMinutes(-10);
        int failedAttempts = await dbContext.SessionAttemptAudits
            .CountAsync(x => x.IpAddress.Equals(request.RequestingIp)
                && !x.Successful
                && x.AttemptDateTime >= cutoffTime,
                cancellationToken);

        if (failedAttempts >= 10)
        {
            // Log the blocked attempt
            await dbContext.SessionAttemptAudits.AddAsync(new SessionAttemptAudit
            {
                IpAddress = request.RequestingIp,
                SessionId = request.SessionId,
                Successful = false,
                AttemptDateTime = DateTimeOffset.UtcNow
            }, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            return QueryResponse<BonesUserSession?>.Fail("Too many failed attempts. Please try again later.");
        }

        BonesUserSession? session = await dbContext.UserSessions.FindAsync([request.SessionId], cancellationToken);

        bool successful = session is not null;

        // Log the attempt
        await dbContext.SessionAttemptAudits.AddAsync(new SessionAttemptAudit
        {
            IpAddress = request.RequestingIp,
            SessionId = request.SessionId,
            Successful = successful,
            AttemptDateTime = DateTimeOffset.UtcNow
        }, cancellationToken);

        if (successful && session is not null)
        {
            session.LastAccessedDateTime = DateTimeOffset.UtcNow;
            dbContext.UserSessions.Update(session);
            await dbContext.SaveChangesAsync(cancellationToken);
            return session;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return QueryResponse<BonesUserSession?>.Fail("Session not found.");
    }
}
