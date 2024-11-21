namespace Bones.Database.Operations.SystemQueues.ForgotPassword.AnyForgotPasswordEmailsInQueueDb;

internal sealed class AnyForgotPasswordEmailsInQueueDbHandler(BonesDbContext dbContext) : IRequestHandler<AnyForgotPasswordEmailsInQueueDbQuery, QueryResponse<bool>>
{
    public async Task<QueryResponse<bool>> Handle(AnyForgotPasswordEmailsInQueueDbQuery request, CancellationToken cancellationToken)
    {
        return await dbContext.ForgotPasswordEmailQueue.AnyAsync(cancellationToken);
    }
}