using Bones.Database.DbSets;
using Bones.Database.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bones.Database.Operations.Tasks;

/// <summary>
///     DB Query for getting all unclosed task history entry IDs.
/// </summary>
public record GetUnclosedTaskHistoriesDbQuery : IRequest<DbQueryResponse<List<Guid>>>;

internal class GetUnclosedTaskHistoriesDbHandler(BonesDbContext dbContext)
    : IRequestHandler<GetUnclosedTaskHistoriesDbQuery, DbQueryResponse<List<Guid>>>
{
    public async Task<DbQueryResponse<List<Guid>>> Handle(GetUnclosedTaskHistoriesDbQuery request,
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