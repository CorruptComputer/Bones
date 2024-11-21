using Bones.Database.DbSets.SystemQueues;
using Bones.Shared.Extensions;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Bones.Database.Operations.SystemQueues.ConfirmationEmail.AddConfirmationEmailToQueueDb;

internal sealed class AddConfirmationEmailToQueueDbHandler(BonesDbContext dbContext) : IRequestHandler<AddConfirmationEmailToQueueDbCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(AddConfirmationEmailToQueueDbCommand request, CancellationToken cancellationToken)
    {
        if (!await request.EmailTo.IsValidEmailAsync(cancellationToken))
        {
            return CommandResponse.Fail("Invalid email address");
        }

        if (await dbContext.ConfirmationEmailQueue.AnyAsync(x => x.EmailTo == request.EmailTo, cancellationToken))
        {
            return CommandResponse.Fail("Email address already in queue for confirmation");
        }

        EntityEntry<ConfirmationEmailQueue> created = await dbContext.ConfirmationEmailQueue.AddAsync(new()
        {
            EmailTo = request.EmailTo,
            ConfirmationLink = request.ConfirmationLink
        }, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass(created.Entity.Id);
    }
}