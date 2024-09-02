using Bones.Database.DbSets.ProjectManagement;
using Bones.Database.Operations.ProjectManagement.Queues.QueueDeleteQueueByIdDb;

namespace Bones.Database.Operations.ProjectManagement.Initiatives.QueueDeleteInitiativeByIdDb;

internal sealed class QueueDeleteInitiativeByIdDbHandler(BonesDbContext dbContext, ISender sender) : IRequestHandler<QueueDeleteInitiativeByIdDbCommand, CommandResponse>
{
    /// <inheritdoc />
    public async Task<CommandResponse> Handle(QueueDeleteInitiativeByIdDbCommand request, CancellationToken cancellationToken)
    {
        Initiative? initiative = await dbContext.Initiatives.Include(initiative =>  initiative.Queues).FirstOrDefaultAsync(i => i.Id == request.InitiativeId, cancellationToken);
        if (initiative == null)
        {
            return CommandResponse.Fail("Invalid ProjectId.");
        }

        foreach (Queue queue in initiative.Queues)
        {
            await sender.Send(new QueueDeleteQueueByIdDbCommand(queue.Id), cancellationToken);
        }

        initiative.DeleteFlag = true;
        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass();
    }
}