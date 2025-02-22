using Bones.Database.DbSets.System;

namespace Bones.Database.Operations.System.Queues;

/// <inheritdoc />
public sealed class GetForgotPasswordEmailsInQueueDb(BonesDbContext dbContext) : IRequestHandler<GetForgotPasswordEmailsInQueueDb.Query, QueryResponse<List<ForgotPasswordEmailQueue>>>
{
    /// <summary>
    ///   Checks if any confirmation emails are in the queue
    /// </summary>
    public sealed record Query : IRequest<QueryResponse<List<ForgotPasswordEmailQueue>>>;

    /// <inheritdoc />
    public async Task<QueryResponse<List<ForgotPasswordEmailQueue>>> Handle(Query request, CancellationToken cancellationToken)
    {
        return await dbContext.ForgotPasswordEmailQueue.ToListAsync(cancellationToken);
    }
}