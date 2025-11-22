using Bones.Database.DbSets.TaskManagement;

namespace Bones.Database.Operations.TaskManagement.TaskQueues;

/// <inheritdoc />
public sealed class UpdateQueueByIdDb(BonesDbContext dbContext) : IRequestHandler<UpdateQueueByIdDb.Command, CommandResponse>
{
    /// <summary>
    ///   DB Command for updating a Queue.
    /// </summary>
    /// <param name="QueueId">Internal ID of the queue</param>
    /// <param name="NewName">The new name of the queue</param>
    public sealed record Command(Guid QueueId, string NewName) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.QueueId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.NewName).NotEmpty();
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        TaskQueue? queue = await dbContext.TaskQueues.FirstOrDefaultAsync(p => p.Id == request.QueueId, cancellationToken);
        if (queue == null)
        {
            return CommandResponse.Fail("Invalid QueueId.");
        }

        queue.Name = request.NewName;

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass();
    }
}
