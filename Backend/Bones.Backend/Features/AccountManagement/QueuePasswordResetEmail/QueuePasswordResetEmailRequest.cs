using System.ComponentModel.DataAnnotations;
using Bones.Database.DbSets.AccountManagement;
using Bones.Shared.Backend.Models;

namespace Bones.Backend.Features.AccountManagement.QueuePasswordResetEmail;

public sealed record QueuePasswordResetRequest([Required] BonesUser User, [Required] string Email, [Required] string Code) : IRequest<CommandResponse>;