using Bones.Database.DbSets.TaskManagement;
using Bones.Database.Operations.TaskManagement.Tasks;

namespace Bones.Database.Operations.TaskManagement.TaskQueues;

/// <inheritdoc />
public sealed class QueueDeleteQueueByIdDb(BonesDbContext dbContext, ISender sender) : IRequestHandler<QueueDeleteQueueByIdDb.Command, CommandResponse>
{
    /// <summary>
    ///   DB Command for deleting a Queue.
    /// </summary>
    /// <param name="QueueId">Internal ID of the queue</param>
    public sealed record Command(Guid QueueId) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.QueueId).NotNull().NotEqual(Guid.Empty);
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        TaskQueue? queue = await dbContext.TaskQueues
            .Include(queue => queue.Tasks)
            .FirstOrDefaultAsync(p => p.Id == request.QueueId, cancellationToken);

        if (queue == null)
        {
            return CommandResponse.Fail("Invalid QueueId.");
        }

        foreach (BonesTask item in queue.Tasks)
        {
            // TODO: Might want to eventually add the ability to move these to a different queue instead
            await sender.Send(new QueueDeleteTaskByIdDb.Command(item.Id), cancellationToken);
        }

        queue.DeleteFlag = true;
        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass();
    }
}