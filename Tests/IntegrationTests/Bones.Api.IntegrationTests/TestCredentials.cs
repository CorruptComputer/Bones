namespace Bones.Api.IntegrationTests;

internal class TestCredentials
{
    internal enum User
    {
        DefaultAdmin,
        Invalid,
        Unconfirmed,
        ChangeEmail,
        ChangePassword
    }

    internal static Dictionary<User, (string email, string password)> Credentials = new()
    {
        // This one is added by default when the database is created, the rest should be created by the tests if needed
        { User.DefaultAdmin, ("admin@example.com", "ChangeMe1!") },
        { User.Invalid, ("invalid@example.com", "InvalidPassword1!") },
        { User.Unconfirmed, ("unconfirmed@example.com", "ChangeMe1!") },
        { User.ChangeEmail, ("change-email@example.com", "Example1!") },
        { User.ChangePassword, ("change-password@example.com", "Example1!") },
    };
}
