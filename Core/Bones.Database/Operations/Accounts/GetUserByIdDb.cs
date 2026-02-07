using Bones.Database.DbSets.Accounts;

namespace Bones.Database.Operations.Accounts;

/// <inheritdoc />
public class GetUserByIdDb(BonesDbContext dbContext) : IRequestHandler<GetUserByIdDb.Query, QueryResponse<BonesUser?>>
{
    /// <summary>
    ///   DB Query for getting a user by ID
    /// </summary>
    /// <param name="UserId"></param>
    public sealed record Query(Guid UserId) : IRequest<QueryResponse<BonesUser?>>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Query>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.UserId).NotEmpty().NotEqual(Guid.Empty);
        }
    }

    /// <inheritdoc />
    public async Task<QueryResponse<BonesUser?>> Handle(Query request, CancellationToken cancellationToken)
    {
        return await dbContext.Users.FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);
    }
}