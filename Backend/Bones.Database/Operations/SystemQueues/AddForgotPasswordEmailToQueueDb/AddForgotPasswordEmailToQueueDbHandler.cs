using Bones.Database.DbSets.SystemQueues;
using Bones.Shared.Extensions;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Bones.Database.Operations.SystemQueues.AddForgotPasswordEmailToQueueDb;

internal sealed class AddForgotPasswordEmailToQueueDbHandler(BonesDbContext dbContext) : IRequestHandler<AddForgotPasswordEmailToQueueDbCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(AddForgotPasswordEmailToQueueDbCommand request, CancellationToken cancellationToken)
    {
        if (!await request.EmailTo.IsValidEmailAsync(cancellationToken))
        {
            return CommandResponse.Fail("Invalid email address");
        }

        if (await dbContext.ForgotPasswordEmailQueue.AnyAsync(x => x.EmailTo == request.EmailTo, cancellationToken))
        {
            return CommandResponse.Fail("Email address already in queue for reset");
        }

        EntityEntry<ForgotPasswordEmailQueue> created = await dbContext.ForgotPasswordEmailQueue.AddAsync(new()
        {
            EmailTo = request.EmailTo,
            PasswordResetLink = request.PasswordResetLink
        }, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass(created.Entity.Id);
    }
}