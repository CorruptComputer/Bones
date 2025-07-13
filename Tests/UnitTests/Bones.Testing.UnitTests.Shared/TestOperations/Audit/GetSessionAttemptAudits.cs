using System.Net;
using Bones.Database;
using Bones.Database.DbSets.Audit;
using Bones.Shared.Backend.Models;
using Questy;
using Microsoft.EntityFrameworkCore;

namespace Bones.Testing.UnitTests.Shared.TestOperations.Audit;

/// <inheritdoc />
public class GetSessionAttemptAudits(BonesDbContext dbContext) : IRequestHandler<GetSessionAttemptAudits.Query, QueryResponse<List<SessionAttemptAudit>>>
{
    /// <summary>
    ///   TESTING QUERY: Get all session attempt audits for an IP
    /// </summary>
    /// <param name="IpAddress"></param>
    public record Query(IPAddress IpAddress) : IRequest<QueryResponse<List<SessionAttemptAudit>>>;

    /// <inheritdoc />
    public async Task<QueryResponse<List<SessionAttemptAudit>>> Handle(Query request, CancellationToken cancellationToken)
    {
        List<SessionAttemptAudit> audits = await dbContext.SessionAttemptAudits
            .Where(x => x.IpAddress.Equals(request.IpAddress))
            .OrderBy(x => x.AttemptDateTime)
            .ToListAsync(cancellationToken);

        return audits;
    }
}