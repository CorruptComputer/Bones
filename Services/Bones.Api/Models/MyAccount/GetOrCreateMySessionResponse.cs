using Bones.Database.DbSets.Accounts;

namespace Bones.Api.Models.MyAccount;

/// <summary>
///   Response for the GetOrCreateMySessionAsync endpoint
/// </summary>
public sealed record GetOrCreateMySessionResponse
{
    /// <summary>
    ///   The session SessionId to use
    /// </summary>
    public required Guid SessionId { get; init; }

    /// <summary>
    ///   The encryption key to use
    /// </summary>
    public required string Base64LocalStorageKey { get; init; }

    internal static GetOrCreateMySessionResponse FromSession(BonesUserSession session)
    {
        return new GetOrCreateMySessionResponse
        {
            SessionId = session.Id,
            Base64LocalStorageKey = session.Base64LocalStorageKey
        };
    }
}
