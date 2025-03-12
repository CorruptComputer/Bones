using Bones.Database.DbSets.AccountManagement;
using Bones.Shared.Backend.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Bones.Testing.Shared.Backend.TestOperations.AccountManagement;

/// <inheritdoc />
public class GetAllUsers(UserManager<BonesUser> userManager) : IRequestHandler<GetAllUsers.Query, QueryResponse<List<BonesUser>>>
{
    /// <summary>
    ///   TESTING QUERY: Get all users
    /// </summary>
    public record Query : IRequest<QueryResponse<List<BonesUser>>>;

    /// <inheritdoc />
    public async Task<QueryResponse<List<BonesUser>>> Handle(Query request, CancellationToken cancellationToken)
    {
        List<BonesUser> users = await userManager.Users.ToListAsync(cancellationToken);
        return users;
    }
}
