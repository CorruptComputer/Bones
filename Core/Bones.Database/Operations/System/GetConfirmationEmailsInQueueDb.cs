using Bones.Database.DbSets.System;

namespace Bones.Database.Operations.System;

/// <inheritdoc />
public sealed class GetConfirmationEmailsInQueueDb(BonesDbContext dbContext) : IRequestHandler<GetConfirmationEmailsInQueueDb.Query, QueryResponse<List<ConfirmationEmailQueue>>>
{
    /// <summary>
    ///   Checks if any confirmation emails are in the queue
    /// </summary>
    public sealed record Query : IRequest<QueryResponse<List<ConfirmationEmailQueue>>>;

    /// <inheritdoc />
    public async Task<QueryResponse<List<ConfirmationEmailQueue>>> Handle(Query request, CancellationToken cancellationToken)
    {
        return await dbContext.ConfirmationEmailQueue.ToListAsync(cancellationToken);
    }
}