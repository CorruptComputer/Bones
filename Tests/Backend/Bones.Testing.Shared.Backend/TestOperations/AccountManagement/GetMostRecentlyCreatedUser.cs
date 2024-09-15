using Bones.Database.DbSets.AccountManagement;
using Bones.Shared.Backend.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Bones.Testing.Shared.Backend.TestOperations.AccountManagement;


public record GetMostRecentlyCreatedUserQuery : IRequest<QueryResponse<BonesUser>>;

public class GetMostRecentlyCreatedUser(UserManager<BonesUser> userManager) : IRequestHandler<GetMostRecentlyCreatedUserQuery, QueryResponse<BonesUser>>
{
    public async Task<QueryResponse<BonesUser>> Handle(GetMostRecentlyCreatedUserQuery request, CancellationToken cancellationToken)
    {
        BonesUser? user = await userManager.Users.OrderByDescending(x => x.CreateDateTime)
            .FirstOrDefaultAsync(cancellationToken);
        return user;
    }
}
