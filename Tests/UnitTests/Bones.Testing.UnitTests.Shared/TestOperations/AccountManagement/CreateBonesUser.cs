using Bones.Database.DbSets.AccountManagement;
using Bones.Shared.Backend.Models;
using Questy;
using Microsoft.AspNetCore.Identity;

namespace Bones.Testing.UnitTests.Shared.TestOperations.AccountManagement;

/// <inheritdoc />
public class CreateBonesUser(UserManager<BonesUser> userManager) : IRequestHandler<CreateBonesUser.Query, QueryResponse<IdentityResult>>
{
    /// <summary>
    ///   TESTING QUERY: Create a user for testing
    /// </summary>
    /// <param name="Email"></param>
    /// <param name="Password"></param>
    public record Query(string Email, string Password) : IRequest<QueryResponse<IdentityResult>>;

    /// <inheritdoc />
    public async Task<QueryResponse<IdentityResult>> Handle(Query request, CancellationToken cancellationToken)
    {
        BonesUser user = new()
        {
            Email = request.Email,
            UserName = request.Email,
            EmailConfirmed = true
        };

        return await userManager.CreateAsync(user, request.Password);
    }
}