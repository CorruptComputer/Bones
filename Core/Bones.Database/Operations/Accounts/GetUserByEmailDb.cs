using Bones.Database.DbSets.Accounts;

namespace Bones.Database.Operations.Accounts;

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
        // EF can't handle str1.Equals(str2, StringComparison.InvariantCultureIgnoreCase)
        string normalizedEmail = request.Email.ToUpperInvariant();

        return await dbContext.Users.FirstOrDefaultAsync(x => x.NormalizedEmail == normalizedEmail, cancellationToken);
    }
}