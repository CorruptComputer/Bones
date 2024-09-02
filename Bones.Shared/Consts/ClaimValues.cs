namespace Bones.Shared.Consts;

/// <summary>
///   User claims are all additive, so the only value we really have is "yes" if they don't have it they won't have it.
/// </summary>
public static class ClaimValues
{
    /// <summary>
    ///   They have the claim
    /// </summary>
    public const string YES = "yes";
}