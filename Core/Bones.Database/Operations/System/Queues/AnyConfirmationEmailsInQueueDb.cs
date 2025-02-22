namespace Bones.Database.Operations.System.Queues;

/// <inheritdoc />
public sealed class AnyConfirmationEmailsInQueueDb(BonesDbContext dbContext) : IRequestHandler<AnyConfirmationEmailsInQueueDb.Query, QueryResponse<bool>>
{
    /// <summary>
    ///   Checks if any confirmation emails are in the queue
    /// </summary>
    public sealed record Query : IRequest<QueryResponse<bool>>;

    /// <inheritdoc />
    public async Task<QueryResponse<bool>> Handle(Query request, CancellationToken cancellationToken)
    {
        return await dbContext.ConfirmationEmailQueue.AnyAsync(cancellationToken);
    }
}