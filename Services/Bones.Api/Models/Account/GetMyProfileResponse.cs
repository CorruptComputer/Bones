using System.Security.Claims;
using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.Audit;
using Bones.Shared.Consts;

namespace Bones.Api.Models.Account;

/// <summary>
///   Response for the GetMyProfileAsync endpoint
/// </summary>
[JsonSerializable(typeof(GetMyProfileResponse))]
public sealed record GetMyProfileResponse
{
    /// <summary>
    ///   The email address of the user
    /// </summary>
    [JsonRequired]
    public required string Email { get; init; }

    /// <summary>
    ///   Have they confirmed their email address?
    /// </summary>
    [JsonRequired]
    public required bool EmailConfirmed { get; init; }

    /// <summary>
    ///   If they have confirmed their email address, when they did it
    /// </summary>
    public DateTimeOffset? EmailConfirmedDateTime { get; init; }

    /// <summary>
    ///   The display name of the user, defaults to their email if they don't have one set
    /// </summary>
    [JsonRequired]
    public required string DisplayName { get; init; }

    /// <summary>
    ///   The date and time the user was created
    /// </summary>
    [JsonRequired]
    public required DateTimeOffset CreateDateTime { get; init; }

    /// <summary>
    ///   The date and time the user last set their password
    /// </summary>
    [JsonRequired]
    public required DateTimeOffset PasswordLastSetDateTime { get; init; }

    /// <summary>
    ///   Is the user a system administrator?
    /// </summary>
    [JsonRequired]
    public required bool IsSysAdmin { get; init; }

    /// <summary>
    ///   A list of audits that have been performed on the account
    /// </summary>
    [JsonRequired]
    public required List<MyAccountAuditModel> AccountAudits { get; init; }

    internal static GetMyProfileResponse FromUser(BonesUser user, ClaimsPrincipal claims, List<AccountAudit> audits)
    {
        return new()
        {
            Email = user.Email ?? string.Empty,
            EmailConfirmed = user.EmailConfirmed,
            EmailConfirmedDateTime = user.EmailConfirmedDateTime,
            DisplayName = user.DisplayName ?? "Unknown",
            CreateDateTime = user.CreateDateTime,
            PasswordLastSetDateTime = user.PasswordLastSetDateTime,
            IsSysAdmin = claims.IsInRole(SystemRoles.SYSTEM_ADMINISTRATORS),
            AccountAudits = [.. audits.Select(audit => new MyAccountAuditModel
            {
                DateTime = audit.ActionDateTime,
                Action = audit.ActionTaken.ToString(),
                ActionBy = audit.ActionTakenBy.DisplayName ?? "Unknown"
            })]
        };
    }

    /// <summary>
    ///   Represents an audit of a user account
    /// </summary>
    [JsonSerializable(typeof(MyAccountAuditModel))]
    public sealed record MyAccountAuditModel
    {
        /// <summary>
        ///   The date and time the action was taken
        /// </summary>
        [JsonRequired]
        public required DateTimeOffset DateTime { get; init; }

        /// <summary>
        ///   The action that took place
        /// </summary>
        [JsonRequired]
        public required string Action { get; init; }

        /// <summary>
        ///   Who is responsible for the action
        /// </summary>
        [JsonRequired]
        public required string ActionBy { get; init; }
    }
}
