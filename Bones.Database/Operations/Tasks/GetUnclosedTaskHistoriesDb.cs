using Bones.Database.DbSets;
using Microsoft.EntityFrameworkCore;

namespace Bones.Database.Operations.Tasks;

/// <summary>
///     DB Query for getting all unclosed task history entry IDs.
/// </summary>
public record GetUnclosedTaskHistoriesDbQuery : IRequest<QueryResponse<List<Guid>>>;

internal class GetUnclosedTaskHistoriesDbHandler(BonesDbContext dbContext)
    : IRequestHandler<GetUnclosedTaskHistoriesDbQuery, QueryResponse<List<Guid>>>
{
    public async Task<QueryResponse<List<Guid>>> Handle(GetUnclosedTaskHistoriesDbQuery request,
        CancellationToken cancellationToken)
    {
        IQueryable<TaskHistory> unclosed = dbContext.TaskHistories.Where(ts => ts.EndDateTime == null);

        return new()
        {
            Success = true,
            Result = await unclosed.Select(th => th.Id).ToListAsync(cancellationToken)
        };
    }
}