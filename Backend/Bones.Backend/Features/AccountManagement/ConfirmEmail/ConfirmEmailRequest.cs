using Bones.Shared.Backend.Models;
using Microsoft.AspNetCore.Identity;

namespace Bones.Backend.Features.AccountManagement.ConfirmEmail;

public sealed record ConfirmEmailRequest(Guid UserId, string Code, string? ChangedEmail) : IRequest<QueryResponse<IdentityResult>>;