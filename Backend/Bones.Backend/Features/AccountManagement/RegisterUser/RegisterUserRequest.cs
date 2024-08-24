using Bones.Shared.Backend.Models;
using Microsoft.AspNetCore.Identity;

namespace Bones.Backend.Features.AccountManagement.RegisterUser;

public sealed record RegisterUserRequest(string Email, string Password) : IRequest<QueryResponse<IdentityResult>>;