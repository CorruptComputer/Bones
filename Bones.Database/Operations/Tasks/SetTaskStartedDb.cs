using Bones.Database.DbSets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Bones.Database.Operations.Tasks;

/// <summary>
///     DB Command for setting a task to running and logging its start time.
/// </summary>
/// <param name="TaskName">Name of the task to start</param>
public record SetTaskStartedDbCommand(string TaskName) : IRequest<CommandResponse>;

internal class SetTaskStartedDbHandler(BonesDbContext dbContext)
    : IRequestHandler<SetTaskStartedDbCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(SetTaskStartedDbCommand request, CancellationToken cancellationToken)
    {
        TaskSchedule? schedule =
            await dbContext.TaskSchedules.FirstOrDefaultAsync(ts => ts.TaskName == request.TaskName, cancellationToken);
        if (schedule == null)
        {
            schedule = new()
            {
                TaskName = request.TaskName,
                Running = true
            };

            await dbContext.TaskSchedules.AddAsync(schedule, cancellationToken);
        }
        else
        {
            schedule.Running = true;
        }

        TaskHistory taskHistory = new()
        {
            TaskName = request.TaskName,
            StartDateTime = DateTimeOffset.UtcNow
        };

        EntityEntry<TaskHistory> created = await dbContext.TaskHistories.AddAsync(taskHistory, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new()
        {
            Success = true,
            Id = created.Entity.Id
        };
    }
}