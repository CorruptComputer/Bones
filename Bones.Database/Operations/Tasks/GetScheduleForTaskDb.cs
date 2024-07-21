using Bones.Database.DbSets;
using Microsoft.EntityFrameworkCore;

namespace Bones.Database.Operations.Tasks;

/// <summary>
///     DB Query for getting the schedule for a task.
/// </summary>
/// <param name="TaskName">Name of the task</param>
public record GetScheduleForTaskDbQuery(string TaskName) : IRequest<QueryResponse<GetScheduleForTaskDbResponse>>;

/// <summary>
///     DB Response for getting the schedule for a task.
/// </summary>
/// <param name="LastRunDateTime">Last time the task completed</param>
/// <param name="NextRunDateTime">Time when the task is eligible to run next</param>
/// <param name="Running">Is it currently running</param>
public record GetScheduleForTaskDbResponse(
    DateTimeOffset? LastRunDateTime,
    DateTimeOffset? NextRunDateTime,
    bool Running);

internal class GetScheduleForTaskDbHandler(BonesDbContext dbContext)
    : IRequestHandler<GetScheduleForTaskDbQuery, QueryResponse<GetScheduleForTaskDbResponse>>
{
    public async Task<QueryResponse<GetScheduleForTaskDbResponse>> Handle(GetScheduleForTaskDbQuery request,
        CancellationToken cancellationToken)
    {
        TaskSchedule? schedule =
            await dbContext.TaskSchedules.FirstOrDefaultAsync(ts => ts.TaskName == request.TaskName, cancellationToken);

        return new()
        {
            Success = true,
            Result = new(schedule?.LastRunTime, schedule?.NextRunTime, schedule?.Running ?? false)
        };
    }
}