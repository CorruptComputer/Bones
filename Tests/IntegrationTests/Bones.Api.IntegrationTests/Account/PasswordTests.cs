namespace Bones.Api.IntegrationTests.Account;

/// <summary>
///   Provides tests for the password change functionality of the Bones API.
/// </summary>
public class PasswordTests : TestBase
{
    /// <summary>
    ///   Change password user should be able to change their password
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ChangePasswordUser_ShouldBeAbleToChangePassword()
    {
        await SetupApiClientAsync();
        await LoginAsync(TestCredentials.User.ChangePassword);

        bool? successful = null;
        if (ApiClient is not null)
        {
            Task<bool?> task = ApiClient.MyAccount.Password.PutAsync(new ChangeMyPasswordRequest()
            {
                CurrentPassword = TestCredentials.Credentials[TestCredentials.User.ChangePassword].password,
                InvalidateOtherSessions = false,
                NewPassword = "newPassword!123"
            });
            await task.ShouldNotThrowAsync();
            successful = await task;
        }

        successful.ShouldNotBeNull();
        successful.ShouldBe(true);
    }
}