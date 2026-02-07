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
    ///   Default values for test users created when SetupForTesting is true.
    /// </summary>
    public static class TestUsers
    {
        /// <summary>
        ///   The email address to use for the default test user.
        /// </summary>
        public const string TEST_USER_EMAIL = "user@example.com";

        /// <summary>
        ///   The email address to use for the change email test user.
        /// </summary>
        public const string CHANGE_EMAIL_TEST_USER_EMAIL = "change-email@example.com";

        /// <summary>
        ///   The email address to use for the change password test user.
        /// </summary>
        public const string CHANGE_PASSWORD_TEST_USER_EMAIL = "change-password@example.com";
    }
}
