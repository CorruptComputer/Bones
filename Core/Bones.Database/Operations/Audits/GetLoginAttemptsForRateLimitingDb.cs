using System.Net;

namespace Bones.Database.Operations.Audits;

/// <inheritdoc />
public sealed class GetLoginAttemptsForRateLimitingDb(BonesDbContext dbContext) : IRequestHandler<GetLoginAttemptsForRateLimitingDb.Query, QueryResponse<int>>
{
    /// <summary>
    ///   Gets the number of failed login attempts for an IP address within the cutoff time
    /// </summary>
    /// <param name="RequestingIp"></param>
    /// <param name="CutoffTime"></param>
    public sealed record Query(IPAddress RequestingIp, DateTimeOffset CutoffTime) : IRequest<QueryResponse<int>>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Query>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.RequestingIp).NotNull();
            RuleFor(x => x.CutoffTime).NotNull();
        }
    }

    /// <inheritdoc />
    public async Task<QueryResponse<int>> Handle(Query request, CancellationToken cancellationToken)
    {
        int failedAttempts = await dbContext.LoginAudits
            .CountAsync(x => x.RequestingIpAddress.Equals(request.RequestingIp)
                && !x.Successful
                && x.LoginDateTime >= request.CutoffTime,
                cancellationToken);

        return QueryResponse<int>.Pass(failedAttempts);
    }
}