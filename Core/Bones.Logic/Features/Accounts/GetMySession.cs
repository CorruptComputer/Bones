using System.Net;
using Bones.Database.DbSets.AccountManagement;
using Bones.Database.Operations.AccountManagement;
using Bones.Shared.Backend;

namespace Bones.Logic.Features.Accounts;

/// <inheritdoc />
public sealed class GetMySession(ISender sender) : IRequestHandler<GetMySession.Query, QueryResponse<BonesUserSession?>>
{
    /// <summary>
    ///   Backend request for getting or creating a session for the user
    /// </summary>
    /// <param name="SessionId"></param>
    /// <param name="RequestingIp"></param>
    /// <param name="RequestingUser"></param>
    public sealed record Query(Guid SessionId, IPAddress RequestingIp, BonesUser RequestingUser) : IRequest<QueryResponse<BonesUserSession?>>;

    /// <inheritdoc />
    public class Validator : AbstractValidator<Query>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.SessionId).NotEqual(Guid.Empty);
            RuleFor(x => x.SessionId).NotEqual(Guid.Empty);
            RuleFor(x => x.RequestingUser).NotNull();
        }
    }

    /// <inheritdoc />
    public async Task<QueryResponse<BonesUserSession?>> Handle(Query request, CancellationToken cancellationToken)
    {

        BonesUserSession? session = await sender.Send(new GetBonesUserSessionDb.Query(request.SessionId), cancellationToken);

        // If something funky is happening just invalidate the session and force them to login again
        if (session is not null 
            && (!session.IpAddress.Equals(request.RequestingIp) 
                || session.User.Id != request.RequestingUser.Id))
        {
            // Don't care about the return value, just send it
            _ = sender.Send(new InvalidateBonesUserSessionDb.Command(request.SessionId), cancellationToken);
            session = null;
        }

        return session;
    }
}
