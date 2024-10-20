using Bones.Database.DbSets.ProjectManagement;
using Bones.Database.DbSets.WorkItemManagement;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Bones.Database.Operations.WorkItemManagement.WorkItemQueues.CreateQueueDb;

internal sealed class CreateQueueDbHandler(BonesDbContext dbContext) : IRequestHandler<CreateQueueDbCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(CreateQueueDbCommand request, CancellationToken cancellationToken)
    {
        Initiative? initiative = await dbContext.Initiatives.FirstOrDefaultAsync(i => i.Id == request.InitiativeId, cancellationToken);
        if (initiative == null)
        {
            return CommandResponse.Fail("Invalid InitiativeId.");
        }

        EntityEntry<WorkItemQueue> created = await dbContext.WorkItemQueues.AddAsync(new()
        {
            Name = request.Name,
            Initiative = initiative
        }, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass(created.Entity.Id);
    }
}