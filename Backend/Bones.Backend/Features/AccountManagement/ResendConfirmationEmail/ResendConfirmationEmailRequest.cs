using Bones.Shared.Backend.Models;
using Microsoft.AspNetCore.Identity;

namespace Bones.Backend.Features.AccountManagement.ResendConfirmationEmail;

public sealed record ResendConfirmationEmailRequest(string Email) : IRequest<CommandResponse>;