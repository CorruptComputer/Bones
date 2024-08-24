using Bones.Database.DbSets.AccountManagement;
using Bones.Shared.Backend.Models;

namespace Bones.Backend.Features.AccountManagement.QueueConfirmationEmail;

public sealed record QueueConfirmationEmailRequest(BonesUser User, string Email) : IRequest<CommandResponse>;