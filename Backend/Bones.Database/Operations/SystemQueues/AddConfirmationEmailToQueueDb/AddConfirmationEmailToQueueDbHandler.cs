using Bones.Database.DbSets.SystemQueues;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Bones.Database.Operations.SystemQueues.AddConfirmationEmailToQueue;

internal sealed class AddConfirmationEmailToQueueDbHandler(BonesDbContext dbContext) : IRequestHandler<AddConfirmationEmailToQueueDbCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(AddConfirmationEmailToQueueDbCommand request, CancellationToken cancellationToken)
    {
        EntityEntry<ConfirmationEmailQueue> created = await dbContext.ConfirmationEmailQueue.AddAsync(new()
        {
            EmailTo = request.EmailTo,
            ConfirmationLink = request.ConfirmationLink
        }, cancellationToken);

        return CommandResponse.Pass(created.Entity.Id);
    }
}