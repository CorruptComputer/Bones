using System.Net;
using Bones.Database;
using Bones.Database.DbSets.Audit;
using Bones.Shared.Backend.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bones.Testing.UnitTests.Shared.TestOperations.Audit;

/// <summary>
///   Query handler for retrieving session attempt audits by IP address
/// </summary>
public class GetSessionAttemptAuditsByIp
{
    /// <summary>
    ///   Query to retrieve session attempt audits by IP address
    /// </summary>
    /// <param name="IpAddress">The IP address to search for</param>
    public record Query(IPAddress IpAddress) : IRequest<QueryResponse<List<SessionAttemptAudit>>>;

    /// <summary>
    ///   Handler for retrieving session attempt audits by IP address
    /// </summary>
    public class Handler(BonesDbContext db) : IRequestHandler<Query, QueryResponse<List<SessionAttemptAudit>>>
    {
        /// <summary>
        ///   Handles the query to retrieve session attempt audits by IP address
        /// </summary>
        /// <param name="request">The query request</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A list of session attempt audits for the specified IP address</returns>
        public async Task<QueryResponse<List<SessionAttemptAudit>>> Handle(Query request, CancellationToken cancellationToken)
        {
            List<SessionAttemptAudit> audits = await db.SessionAttemptAudits
                .Where(x => x.IpAddress == request.IpAddress)
                .OrderBy(x => x.AttemptDateTime)
                .ToListAsync(cancellationToken);

            return QueryResponse<List<SessionAttemptAudit>>.Pass(audits);
        }
    }
}