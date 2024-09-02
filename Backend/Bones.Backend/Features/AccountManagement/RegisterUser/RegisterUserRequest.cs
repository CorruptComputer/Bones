using Microsoft.AspNetCore.Identity;

namespace Bones.Backend.Features.AccountManagement.RegisterUser;

/// <summary>
///   Backend request for registering a new user.
/// </summary>
/// <param name="Email">Their email address</param>
/// <param name="Password">Their desired password</param>
public sealed record RegisterUserRequest(string Email, string Password) : IRequest<QueryResponse<IdentityResult>>;