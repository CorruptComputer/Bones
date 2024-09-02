using System.ComponentModel.DataAnnotations;
using Bones.Database.DbSets.AccountManagement;

namespace Bones.Backend.Features.AccountManagement.QueueForgotPasswordEmail;

/// <summary>
///   Backend request for queueing a password reset email.
/// </summary>
/// <param name="User"></param>
/// <param name="Email"></param>
/// <param name="Code"></param>
public sealed record QueueForgotPasswordRequest([Required] BonesUser User, [Required] string Email, [Required] string Code) : IRequest<CommandResponse>;