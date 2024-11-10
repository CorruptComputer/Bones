namespace Bones.Backend.Features.Accounts.ResetPassword;

/// <summary>
///   Request to reset password
/// </summary>
public sealed record ResetPasswordCommand : IRequest<CommandResponse>;