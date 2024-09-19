using System.ComponentModel.DataAnnotations;

namespace Bones.Backend.Features.AccountManagement.QueueForgotPasswordEmail;

/// <summary>
///   Backend request for queueing a password reset email.
/// </summary>
/// <param name="Email"></param>
public sealed record QueueForgotPasswordEmailCommand([Required] string Email) : IRequest<CommandResponse>;