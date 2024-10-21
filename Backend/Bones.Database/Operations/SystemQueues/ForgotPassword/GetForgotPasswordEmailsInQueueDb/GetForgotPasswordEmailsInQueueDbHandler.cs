using Bones.Database.DbSets.SystemQueues;

namespace Bones.Database.Operations.SystemQueues.ForgotPassword.GetConfirmationEmailsInQueueDb;

internal sealed class GetForgotPasswordEmailsInQueueDbHandler(BonesDbContext dbContext) : IRequestHandler<GetForgotPasswordEmailsInQueueDbQuery, QueryResponse<List<ForgotPasswordEmailQueue>>>
{
    public async Task<QueryResponse<List<ForgotPasswordEmailQueue>>> Handle(GetForgotPasswordEmailsInQueueDbQuery request, CancellationToken cancellationToken)
    {
        return await dbContext.ForgotPasswordEmailQueue.ToListAsync(cancellationToken);
    }
}