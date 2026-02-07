using System.Net;
using Bones.Database.Operations.Audits;

namespace Bones.Logic.Features.Accounts;

/// <inheritdoc />
public sealed class CheckLoginRateLimit(ISender sender) : IRequestHandler<CheckLoginRateLimit.Query, QueryResponse<bool>>
{
    /// <summary>
    ///   Backend request for checking if a login attempt should be allowed
    /// </summary>
    /// <param name="RequestingIp"></param>
    public sealed record Query(IPAddress RequestingIp) : IRequest<QueryResponse<bool>>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Query>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.RequestingIp).NotNull();
        }
    }

    /// <inheritdoc />
    public async Task<QueryResponse<bool>> Handle(Query request, CancellationToken cancellationToken)
    {
        DateTimeOffset cutoffTime = DateTimeOffset.UtcNow.AddMinutes(-10);
        int? failedAttempts = await sender.Send(new GetLoginAttemptsForRateLimitingDb.Query(request.RequestingIp, cutoffTime), cancellationToken);

        if (failedAttempts >= 10)
        {
            return QueryResponse<bool>.Pass(false);
        }

        return QueryResponse<bool>.Pass(true);
    }
}