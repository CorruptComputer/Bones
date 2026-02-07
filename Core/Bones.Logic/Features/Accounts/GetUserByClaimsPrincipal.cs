using System.Security.Claims;
using Bones.Database.DbSets.Accounts;
using Microsoft.AspNetCore.Identity;

namespace Bones.Logic.Features.Accounts;

/// <inheritdoc />
public sealed class GetUserByClaimsPrincipal(UserManager<BonesUser> userManager) : IRequestHandler<GetUserByClaimsPrincipal.Query, QueryResponse<BonesUser>>
{
    /// <summary>
    ///   Backend request for getting a <see cref="BonesUser" /> by a <see cref="ClaimsPrincipal" />.
    /// </summary>
    /// <param name="ClaimsPrincipal"></param>
    public sealed record Query(ClaimsPrincipal? ClaimsPrincipal) : IRequest<QueryResponse<BonesUser>>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Query>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.ClaimsPrincipal).NotNull();
        }
    }

    /// <inheritdoc />
    public async Task<QueryResponse<BonesUser>> Handle(Query request, CancellationToken cancellationToken)
    {
        if (request.ClaimsPrincipal != null)
        {
            return await userManager.GetUserAsync(request.ClaimsPrincipal);
        }

        return QueryResponse<BonesUser>.Fail("Claims Principal was null");
    }
}