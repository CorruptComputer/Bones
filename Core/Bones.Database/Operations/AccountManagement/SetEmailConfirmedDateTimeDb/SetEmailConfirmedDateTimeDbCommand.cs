using Bones.Database.DbSets.AccountManagement;

namespace Bones.Database.Operations.AccountManagement.SetEmailConfirmedDateTimeDb;

/// <summary>
///   Sets the email confirmed date time on the user
/// </summary>
/// <param name="User"></param>
/// <param name="ConfirmedDateTime"></param>
public sealed record SetEmailConfirmedDateTimeDbCommand(BonesUser User, DateTimeOffset ConfirmedDateTime) : IRequest<CommandResponse>;