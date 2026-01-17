using System.Net;
using Bones.Database.DbSets.AccountManagement;

namespace Bones.Database.Operations.AccountManagement;

/// <inheritdoc />
public class CreateAndGetBonesUserSessionDb(BonesDbContext dbContext) : IRequestHandler<CreateAndGetBonesUserSessionDb.Query, QueryResponse<BonesUserSession>>
{
    /// <summary>
    ///   DB Query for creating a user session
    /// </summary>
    /// <param name="RequestingUserId"></param>
    /// <param name="RequestingIp"></param>
    /// <param name="Base64LocalStorageKey"></param>
    public sealed record Query(Guid RequestingUserId, IPAddress RequestingIp, string Base64LocalStorageKey) : IRequest<QueryResponse<BonesUserSession>>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Query>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.RequestingUserId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.RequestingIp).NotNull().NotEqual(IPAddress.None);
            RuleFor(x => x.Base64LocalStorageKey).NotNull();
        }
    }

    /// <inheritdoc />
    public async Task<QueryResponse<BonesUserSession>> Handle(Query request, CancellationToken cancellationToken)
    {
        BonesUserSession session = new()
        {
            UserId = request.RequestingUserId,
            IpAddress = request.RequestingIp,
            Base64LocalStorageKey = request.Base64LocalStorageKey,
            CreatedDateTime = DateTimeOffset.UtcNow
        };

        await dbContext.UserSessions.AddAsync(session, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return session;
    }
}
