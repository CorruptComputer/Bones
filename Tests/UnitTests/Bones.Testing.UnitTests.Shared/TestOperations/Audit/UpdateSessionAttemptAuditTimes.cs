using Bones.Database;
using Bones.Database.DbSets.Audit;
using Bones.Shared.Backend.Models;
using Questy;
using Microsoft.EntityFrameworkCore;

namespace Bones.Testing.UnitTests.Shared.TestOperations.Audit;

/// <inheritdoc />
public class UpdateSessionAttemptAuditTimes(BonesDbContext dbContext) : IRequestHandler<UpdateSessionAttemptAuditTimes.Command, CommandResponse>
{
    /// <summary>
    ///   TESTING COMMAND: Update all session attempt audit timestamps to a specific time
    /// </summary>
    /// <param name="NewTime"></param>
    public record Command(DateTimeOffset NewTime) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        List<SessionAttemptAudit> audits = await dbContext.SessionAttemptAudits.ToListAsync(cancellationToken);
        List<SessionAttemptAudit> newAudits = audits.Select(audit => new SessionAttemptAudit
        {
            Id = Guid.NewGuid(),
            IpAddress = audit.IpAddress,
            SessionId = audit.SessionId,
            Successful = audit.Successful,
            AttemptDateTime = request.NewTime
        }).ToList();

        dbContext.SessionAttemptAudits.RemoveRange(audits);
        await dbContext.SessionAttemptAudits.AddRangeAsync(newAudits, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass();
    }
}