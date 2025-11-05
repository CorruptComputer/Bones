using Bones.Database.DbSets.AccountManagement;
using Microsoft.AspNetCore.Identity;

namespace Bones.Logic.Features.Accounts;

/// <inheritdoc />
public sealed class IsPasswordExpired(UserManager<BonesUser> userManager) : IRequestHandler<IsPasswordExpired.Query, QueryResponse<bool>>
{
    /// <summary>
    /// Query to check if a user's password is expired
    /// </summary>
    /// <param name="Email">The email of the user to check</param>
    public sealed record Query(string Email) : IRequest<QueryResponse<bool>>;

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
    public async Task<QueryResponse<bool>> Handle(Query request, CancellationToken cancellationToken)
    {
        BonesUser? user = await userManager.FindByEmailAsync(request.Email);
        return QueryResponse<bool>.Pass(user?.PasswordExpired == true);
    }
}