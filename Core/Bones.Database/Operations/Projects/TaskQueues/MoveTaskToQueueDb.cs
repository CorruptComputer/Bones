using Bones.Database.DbSets.Items.Types;
using Bones.Database.DbSets.Projects;

namespace Bones.Database.Operations.TaskManagement.TaskQueues;

/// <inheritdoc />
public sealed class MoveTaskToQueueDb(BonesDbContext dbContext) : IRequestHandler<MoveTaskToQueueDb.Command, CommandResponse>
{
    /// <summary>
    ///   DB Command for putting a task in a queue.
    /// </summary>
    /// <param name="TaskId"></param>
    /// <param name="TaskQueueId"></param>
    public sealed record Command(Guid TaskId, Guid TaskQueueId) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.TaskId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.TaskQueueId).NotNull().NotEqual(Guid.Empty);
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        BonesTask? task = await dbContext.Tasks.FindAsync([request.TaskId], cancellationToken);
        TaskQueue? taskQueue = await dbContext.TaskQueues.FindAsync([request.TaskQueueId], cancellationToken);

        if (task is null || taskQueue is null)
        {
            return CommandResponse.Fail("Invalid Task or Task Queue ID.");
        }

        task.TaskQueue = taskQueue;
        task.AddedToQueueDateTime = DateTimeOffset.UtcNow;

        dbContext.Tasks.Update(task);

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass();
    }
}
