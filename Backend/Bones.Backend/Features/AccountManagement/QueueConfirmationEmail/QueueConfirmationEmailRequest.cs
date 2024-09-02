using Bones.Database.DbSets.AccountManagement;

namespace Bones.Backend.Features.AccountManagement.QueueConfirmationEmail;

/// <summary>
///   Backend command for queueing a confirmation email.
/// </summary>
/// <param name="User"></param>
/// <param name="Email"></param>
/// <param name="IsChange"></param>
public sealed record QueueConfirmationEmailRequest(BonesUser User, string Email, bool IsChange = false) : IRequest<CommandResponse>;