using Bones.Database;
using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.SystemQueues;
using Bones.Shared.Backend.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Bones.Testing.Shared.Backend.TestOperations.AccountManagement;


public record GetAllUsersQuery : IRequest<QueryResponse<List<BonesUser>>>;

public class GetAllUsers(UserManager<BonesUser> userManager) : IRequestHandler<GetAllUsersQuery, QueryResponse<List<BonesUser>>>
{
    public async Task<QueryResponse<List<BonesUser>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        List<BonesUser> users = await userManager.Users.ToListAsync(cancellationToken);
        return users;
    }
}
