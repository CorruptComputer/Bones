using Bones.Database.DbSets;
using Microsoft.EntityFrameworkCore;

namespace Bones.Database.Operations.Tasks;

/// <summary>
///     DB Command for setting a task history entry as finished.
/// </summary>
/// <param name="TaskHistoryId">ID of the task history entry to update.</param>
/// <param name="NextRunDateTime">Time this task should run next, or null if it should not run again.</param>
/// <param name="ErrorMessage">If the task errored out, why? Also sets the tasks status to failed if not null.</param>
public record SetTaskFinishedDbCommand(Guid TaskHistoryId, DateTimeOffset? NextRunDateTime, string? ErrorMessage = null)
    : IRequest<CommandResponse>;

internal class SetTaskFinishedDbHandler(BonesDbContext dbContext)
    : IRequestHandler<SetTaskFinishedDbCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(SetTaskFinishedDbCommand request, CancellationToken cancellationToken)
    {
        TaskHistory? taskHistory =
            await dbContext.TaskHistories.FirstOrDefaultAsync(th => th.Id == request.TaskHistoryId,
                cancellationToken);

        if (taskHistory == null)
        {
            return new()
            {
                Success = false,
                FailureReason = "Invalid TaskHistoryId"
            };
        }

        taskHistory.Errored = request.ErrorMessage != null;
        taskHistory.ErrorMessage = request.ErrorMessage;
        taskHistory.EndDateTime = DateTimeOffset.UtcNow;

        TaskSchedule? schedule =
            await dbContext.TaskSchedules.FirstOrDefaultAsync(ts => ts.TaskName == taskHistory.TaskName,
                cancellationToken);
        if (schedule == null)
        {
            return new()
            {
                Success = false,
                FailureReason = "Invalid TaskName"
            };
        }

        schedule.LastRunTime = DateTimeOffset.UtcNow;
        schedule.NextRunTime = request.NextRunDateTime;
        schedule.Running = false;

        await dbContext.SaveChangesAsync(cancellationToken);

        return new()
        {
            Success = true
        };
    }
}