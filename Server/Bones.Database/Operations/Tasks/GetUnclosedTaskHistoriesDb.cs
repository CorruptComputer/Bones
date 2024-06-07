using Bones.Database.DbSets;
using Bones.Database.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bones.Database.Operations.Tasks;

/// <summary>
///   DB Query for getting all unclosed task history entry IDs.
/// </summary>
public record GetUnclosedTaskHistoriesDbQuery : IRequest<DbQueryResponse<List<long>>>;

internal class GetUnclosedTaskHistoriesDbHandler(BonesDbContext dbContext) : IRequestHandler<GetUnclosedTaskHistoriesDbQuery, DbQueryResponse<List<long>>>
{
    public async Task<DbQueryResponse<List<long>>> Handle(GetUnclosedTaskHistoriesDbQuery request, CancellationToken cancellationToken)
    {
        IQueryable<TaskHistory> unclosed = dbContext.TaskHistories.Where(ts => ts.EndDateTime == null);

        return new()
        {
            Success = true,
            Result = await unclosed.Select(th => th.TaskHistoryId).ToListAsync(cancellationToken)
        };
    }
}