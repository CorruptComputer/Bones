namespace Bones.Database.DbConsts;

/// <summary>
///   Basic audit action reasons
/// </summary>
public static class AuditActionReasons
{
    /// <summary>
    ///   The action was requested by the user, basic reason if the UI doesn't give them a place to give one
    /// </summary>
    public const string UserRequested = "User requested";

    /// <summary>
    ///   Actions taken by the system during DB setup, usually when the DB is being created but perchance when migrations take place too.
    /// </summary>
    public const string DbSetup = "DB setup";
}
