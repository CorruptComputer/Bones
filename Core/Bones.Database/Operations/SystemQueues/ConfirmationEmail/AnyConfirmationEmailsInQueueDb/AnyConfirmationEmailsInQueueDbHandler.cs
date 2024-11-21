namespace Bones.Database.Operations.SystemQueues.ConfirmationEmail.AnyConfirmationEmailsInQueueDb;

internal sealed class AnyConfirmationEmailsInQueueDbHandler(BonesDbContext dbContext) : IRequestHandler<AnyConfirmationEmailsInQueueDbQuery, QueryResponse<bool>>
{
    public async Task<QueryResponse<bool>> Handle(AnyConfirmationEmailsInQueueDbQuery request, CancellationToken cancellationToken)
    {
        return await dbContext.ConfirmationEmailQueue.AnyAsync(cancellationToken);
    }
}