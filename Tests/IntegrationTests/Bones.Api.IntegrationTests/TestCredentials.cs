namespace Bones.Api.IntegrationTests;

internal class TestCredentials
{
    internal enum User
    {
        DefaultAdmin,
        Invalid,
        Unconfirmed
    }

    internal static Dictionary<User, (string, string)> Credentials = new()
    {
        // This one is added by default when the database is created, the rest should be created by the tests if needed
        { User.DefaultAdmin, ("admin@example.com", "ChangeMe1!") },
        { User.Invalid, ("invalid@example.com", "InvalidPassword1!") },
        { User.Unconfirmed, ("unconfirmed@example.com", "ChangeMe1!") }
    };
}
