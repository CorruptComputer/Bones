using Microsoft.AspNetCore.Identity;

namespace Bones.Backend.Features.AccountManagement.ConfirmEmail;

/// <summary>
///   Backend request for confirming a users email
/// </summary>
/// <param name="UserId"></param>
/// <param name="Code"></param>
/// <param name="ChangedEmail"></param>
public sealed record ConfirmEmailRequest(Guid UserId, string Code, string? ChangedEmail) : IRequest<QueryResponse<IdentityResult>>;