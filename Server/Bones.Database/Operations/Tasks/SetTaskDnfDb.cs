using Bones.Database.DbSets;
using Bones.Database.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bones.Database.Operations.Tasks;

/// <summary>
///     DB Command for setting a task history entry to DNF.
/// </summary>
/// <param name="TaskHistoryId">ID of the History to DNF</param>
public record SetTaskDnfDbCommand(Guid TaskHistoryId) : IRequest<DbCommandResponse>;

internal class SetTaskDnfDbHandler(BonesDbContext dbContext) : IRequestHandler<SetTaskDnfDbCommand, DbCommandResponse>
{
    public async Task<DbCommandResponse> Handle(SetTaskDnfDbCommand request, CancellationToken cancellationToken)
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

        taskHistory.Errored = true;
        taskHistory.ErrorMessage = "DNF";
        taskHistory.EndDateTime = DateTimeOffset.MaxValue;

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

        schedule.Running = false;

        await dbContext.SaveChangesAsync(cancellationToken);

        return new()
        {
            Success = true
        };
    }
}