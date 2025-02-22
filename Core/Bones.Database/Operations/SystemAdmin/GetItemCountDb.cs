namespace Bones.Database.Operations.SystemAdmin;

/// <inheritdoc />
public class GetItemCountDb(BonesDbContext dbContext) : IRequestHandler<GetItemCountDb.Query, QueryResponse<int>>
{
    /// <summary>
    ///   Checks if any confirmation emails are in the queue
    /// </summary>
    public sealed record Query : IRequest<QueryResponse<int>>;

    /// <inheritdoc />
    public async Task<QueryResponse<int>> Handle(Query request, CancellationToken cancellationToken)
    {
        return await dbContext.Items.CountAsync(cancellationToken);
    }
}
