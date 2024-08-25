using Bones.Shared.Backend.Models;

namespace Bones.Backend.Features.AccountManagement.ResendConfirmationEmail;

public sealed record ResendConfirmationEmailRequest(string Email) : IRequest<CommandResponse>;