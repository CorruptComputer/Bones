using Bones.Database.DbSets.SystemQueues;

namespace Bones.Database.Operations.SystemQueues.ConfirmationEmail.GetConfirmationEmailsInQueueDb;

internal sealed class GetConfirmationEmailsInQueueDbHandler(BonesDbContext dbContext) : IRequestHandler<GetConfirmationEmailsInQueueDbQuery, QueryResponse<List<ConfirmationEmailQueue>>>
{
    public async Task<QueryResponse<List<ConfirmationEmailQueue>>> Handle(GetConfirmationEmailsInQueueDbQuery request, CancellationToken cancellationToken)
    {
        return await dbContext.ConfirmationEmailQueue.ToListAsync(cancellationToken);
    }
}