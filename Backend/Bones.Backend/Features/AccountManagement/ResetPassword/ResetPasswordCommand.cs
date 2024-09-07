namespace Bones.Backend.Features.AccountManagement.ResetPassword;

/// <summary>
///   Request to reset password
/// </summary>
public sealed record ResetPasswordCommand : IRequest<CommandResponse>;