namespace Bones.Database.Operations.SystemQueues.ForgotPassword.AddForgotPasswordEmailToQueueDb;

/// <summary>
///   Adds an email confirmation to the queue
/// </summary>
/// <param name="EmailTo">The email being confirmed and the email will be sent to</param>
/// <param name="PasswordResetLink">The link for them to click</param>
public sealed record AddForgotPasswordEmailToQueueDbCommand(string EmailTo, string PasswordResetLink) : IRequest<CommandResponse>;