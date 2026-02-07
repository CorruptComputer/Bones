using Bones.Database.DbSets.Accounts;
using Bones.Shared.Backend.Models;
using Questy;
using Microsoft.AspNetCore.Identity;

namespace Bones.Testing.UnitTests.Shared.TestOperations.AccountManagement;

/// <inheritdoc />
public class CreateBonesUser(UserManager<BonesUser> userManager) : IRequestHandler<CreateBonesUser.Query, QueryResponse<BonesUser>>
{
    /// <summary>
    ///   TESTING QUERY: Create a user for testing
    /// </summary>
    /// <param name="Email"></param>
    public record Query(string Email) : IRequest<QueryResponse<BonesUser>>;

    /// <inheritdoc />
    public async Task<QueryResponse<BonesUser>> Handle(Query request, CancellationToken cancellationToken)
    {
        BonesUser user = new()
        {
            Email = request.Email,
            UserName = request.Email,
            EmailConfirmed = true
        };

        // Its for unit tests, doesn't really matter what the password is, it will probably not even be used
        await userManager.CreateAsync(user, "Password123!");

        BonesUser? createdUser = await userManager.FindByEmailAsync(request.Email);

        return createdUser is null
            ? QueryResponse<BonesUser>.Fail("User creation failed")
            : createdUser;
    }
}