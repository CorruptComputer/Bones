using Bones.Database.DbSets.AccountManagement;

namespace Bones.Database.Operations.AccountManagement;

/// <inheritdoc />
public class GetUserByEmailDb(BonesDbContext dbContext) : IRequestHandler<GetUserByEmailDb.Query, QueryResponse<BonesUser?>>
{
    /// <summary>
    ///   DB Query for getting a user by email
    /// </summary>
    /// <param name="Email"></param>
    public sealed record Query(string Email) : IRequest<QueryResponse<BonesUser?>>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Query>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
        }
    }

    /// <inheritdoc />
    public async Task<QueryResponse<BonesUser?>> Handle(Query request, CancellationToken cancellationToken)
    {
        return await dbContext.Users.FirstOrDefaultAsync(x => x.Email == request.Email, cancellationToken);
    }
}