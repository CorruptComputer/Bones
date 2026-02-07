using Bones.Database.DbSets.Projects;
using Bones.Database.Operations.TaskManagement.TaskQueues;

namespace Bones.Database.Operations.Projects.Initiatives;

/// <inheritdoc />
public sealed class QueueDeleteInitiativeByIdDb(BonesDbContext dbContext, ISender sender) : IRequestHandler<QueueDeleteInitiativeByIdDb.Command, CommandResponse>
{
    /// <summary>
    ///   DB Command for queueing an initiative for deletion
    /// </summary>
    /// <param name="InitiativeId"></param>
    public sealed record Command(Guid InitiativeId) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.InitiativeId).NotNull().NotEqual(Guid.Empty);
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        Initiative? initiative = await dbContext.Initiatives.Include(initiative => initiative.Queues).FirstOrDefaultAsync(i => i.Id == request.InitiativeId, cancellationToken);
        if (initiative == null)
        {
            return CommandResponse.Fail("Invalid ProjectId.");
        }

        foreach (TaskQueue queue in initiative.Queues)
        {
            await sender.Send(new QueueDeleteQueueByIdDb.Command(queue.Id), cancellationToken);
        }

        initiative.DeleteFlag = true;
        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass();
    }
}