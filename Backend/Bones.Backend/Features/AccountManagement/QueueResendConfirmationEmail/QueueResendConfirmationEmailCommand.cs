namespace Bones.Backend.Features.AccountManagement.QueueResendConfirmationEmail;

/// <summary>
///   Backend request for resending the confirmation email.
/// </summary>
/// <param name="Email">The email address to resent the confirmation request to.</param>
public sealed record QueueResendConfirmationEmailCommand(string Email) : IRequest<CommandResponse>;