using System.Security.Claims;
using Bones.Database.DbSets.AccountManagement;
using Microsoft.AspNetCore.Identity;

namespace Bones.Logic.Features.Accounts;

/// <summary>
///   Backend request for getting a <see cref="BonesUser" /> by a <see cref="ClaimsPrincipal" />.
/// </summary>
/// <param name="ClaimsPrincipal"></param>
public sealed record GetUserByClaimsPrincipalQuery(ClaimsPrincipal? ClaimsPrincipal) : IRequest<QueryResponse<BonesUser>>;

internal sealed class GetUserByClaimsPrincipalQueryValidator : AbstractValidator<GetUserByClaimsPrincipalQuery>
{
    public GetUserByClaimsPrincipalQueryValidator()
    {
        RuleFor(x => x.ClaimsPrincipal).NotNull();
    }
}

internal sealed class GetUserByClaimsPrincipalHandler(UserManager<BonesUser> userManager) : IRequestHandler<GetUserByClaimsPrincipalQuery, QueryResponse<BonesUser>>
{
    public async Task<QueryResponse<BonesUser>> Handle(GetUserByClaimsPrincipalQuery request, CancellationToken cancellationToken)
    {
        if (request.ClaimsPrincipal != null)
        {
            return await userManager.GetUserAsync(request.ClaimsPrincipal);
        }

        return QueryResponse<BonesUser>.Fail("Claims Principal was null");
    }
}