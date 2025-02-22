namespace Bones.Database.Operations.SystemAdmin;

/// <inheritdoc />
public class GetUserCountDb(BonesDbContext dbContext) : IRequestHandler<GetUserCountDb.Query, QueryResponse<int>>
{
    /// <summary>
    ///   Checks if any confirmation emails are in the queue
    /// </summary>
    public sealed record Query : IRequest<QueryResponse<int>>;

    /// <inheritdoc />
    public async Task<QueryResponse<int>> Handle(Query request, CancellationToken cancellationToken)
    {
        return await dbContext.Users.CountAsync(cancellationToken);
    }
}