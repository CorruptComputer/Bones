using Bones.Database.DbSets.AccountManagement;
using Bones.Logic.Features.Accounts;

namespace Bones.Api.Models.Account;

/// <summary>
///   Request for the ChangeMyEmailAsync endpoint
/// </summary>
[JsonSerializable(typeof(ChangeMyEmailRequest))]
public record ChangeMyEmailRequest
{
    /// <summary>
    ///   The new email address to set for the user
    /// </summary>
    [JsonRequired]
    public required string NewEmail { get; init; }

    internal ChangeEmail.Command ToInternal(BonesUser user)
    {
        return new(NewEmail, user);
    }
}