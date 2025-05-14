using System;

namespace Bones.Database.DbConsts;

/// <summary>
///   Default values for various database entities.
/// </summary>
public static class DefaultValues
{
    /// <summary>
    ///   The default email to use for the system admin user when the database is created.
    /// </summary>
    public const string DEFAULT_SYSTEM_ADMIN_EMAIL = "admin@example.com";

    /// <summary>
    ///   The email address to use for the test user when the database is created and SetupForTesting is true. 
    /// </summary>
    public const string TEST_USER_EMAIL = "user@example.com";
}
