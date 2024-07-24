using Bones.Database.DbSets.Identity;

namespace Bones.Database.Operations.Identity;

public class GetUserByIdDb(BonesDbContext dbContext) : IRequestHandler<GetUserByIdDb.Query, QueryResponse<BonesUser>>
{
    /// <summary>
    ///     DB Query to get a user by UserId
    /// </summary>
    /// <param name="UserId">ID of the User to get</param>
    public record Query(Guid UserId) : IRequest<QueryResponse<BonesUser>>;
    
    public async Task<QueryResponse<BonesUser>> Handle(Query request, CancellationToken cancellationToken)
    {
        return await dbContext.Users.FirstOrDefaultAsync(a => a.Id == request.UserId, cancellationToken);
    }
}