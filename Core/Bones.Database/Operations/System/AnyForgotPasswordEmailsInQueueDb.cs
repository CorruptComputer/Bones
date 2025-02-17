namespace Bones.Database.Operations.System;

/// <inheritdoc />
public sealed class AnyForgotPasswordEmailsInQueueDb(BonesDbContext dbContext) : IRequestHandler<AnyForgotPasswordEmailsInQueueDb.Query, QueryResponse<bool>>
{
    /// <summary>
    ///   Checks if any confirmation emails are in the queue
    /// </summary>
    public sealed record Query : IRequest<QueryResponse<bool>>;

    /// <inheritdoc />
    public async Task<QueryResponse<bool>> Handle(Query request, CancellationToken cancellationToken)
    {
        return await dbContext.ForgotPasswordEmailQueue.AnyAsync(cancellationToken);
    }
}